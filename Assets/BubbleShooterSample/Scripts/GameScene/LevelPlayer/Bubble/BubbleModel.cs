using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public struct BubbleTilePathNode
    {
        public Vector2Int PrevTileIndex;
        public Vector3 PrevTilePosition;
        public Vector2Int NextTileIndex;
        public Vector3 NextTilePosition;
        public int Turn;

        public BubbleTilePathNode(Vector2Int prevTileIndex, Vector3 prevTilePosition, Vector2Int nextTileIndex, Vector3 nextTilePosition, int turn)
        {
            PrevTileIndex = prevTileIndex;
            PrevTilePosition = prevTilePosition;
            NextTileIndex = nextTileIndex;
            NextTilePosition = nextTilePosition;
            Turn = turn;
        }
    }

    public interface IBubbleTileModel
    {
        public Vector2Int TileIndex { get; }
        public Color TileColor { get; }
        public Color BubbleColor { get; }
        public int AttackPoint { get; }
        public BubbleTilePathNode? FirstPathNode { get; }
        public IEnumerable<BubbleTilePathNode> PathNodeQueue { get; }
        public int TileID { get; }

        public void ConsumePathNodeList();
    }

    public class BubbleTileModel : IBubbleTileModel
    {
        public Vector2Int TileIndex => _tileIndex;
        public Color TileColor => _tileColor;
        public Color BubbleColor => _bubbleColor;
        public int AttackPoint => _attackPoint;
        public BubbleTilePathNode? FirstPathNode
        {
            get
            {
                BubbleTilePathNode? result = null;
                if (_pathNodeQueue != null
                    && _pathNodeQueue.Count > 0)
                {
                    result = _pathNodeQueue.Peek();
                }
                return result;
            }
        }
        public IEnumerable<BubbleTilePathNode> PathNodeQueue => _pathNodeQueue;
        public LinkedListNode<IFlowTileModel> NextFlowTileNode
        {
            get
            {
                LinkedListNode<IFlowTileModel> result = null;
                if (_flowTileNode != null)
                {
                    result = _flowTileNode.Next;
                }
                return result;
            }
        }
        public int TileID => _tileID;

        private static int NextID = 0;

        private Vector2Int _tileIndex;
        private Vector2 _tilePosition;
        private Color _tileColor;
        private Color _bubbleColor;

        private Queue<BubbleTilePathNode> _pathNodeQueue;
        private LinkedListNode<IFlowTileModel> _flowTileNode;
        private int _attackPoint;
        private int _tileID;

        // A-1. BubbleShooter 에서 쏘아올린 버블
        public BubbleTileModel(Vector2Int tileIndex, Color bubbleColor)
        {
            _tileIndex = tileIndex;
            _bubbleColor = bubbleColor;
            _tileID = NextID++;
        }

        // A-2. FlowTileModel 로부터 생성되는 버블
        public BubbleTileModel(Vector2Int tileIndex, Vector2 tilePosition, Color tileColor, Color bubbleColor, int attackPoint, int turn, LinkedListNode<IFlowTileModel> flowTileNode)
        {
            _tileIndex = tileIndex;
            _tilePosition = tilePosition;
            _tileColor = tileColor;
            _bubbleColor = bubbleColor;

            _pathNodeQueue = new();
            _pathNodeQueue.Enqueue(new BubbleTilePathNode(tileIndex, tilePosition, tileIndex, tilePosition, turn));
            _flowTileNode = flowTileNode;
            _attackPoint = attackPoint;

            _tileID = NextID++;
        }

        public void MoveToNext(int turn)
        {
            if (_flowTileNode != null)
            {
                _flowTileNode = _flowTileNode.Next;
            }

            if (_flowTileNode != null)
            {
                Vector2Int prevTileIndex = _tileIndex;
                Vector2 prevTilePosition = _tilePosition;
                _tileIndex = _flowTileNode.Value.TileIndex;
                _tilePosition = _flowTileNode.Value.TilePosition;
                _pathNodeQueue.Enqueue(new BubbleTilePathNode(prevTileIndex, prevTilePosition, _tileIndex, _tilePosition, turn));
            }
        }

        public void ConsumePathNodeList()
        {
            _pathNodeQueue.Clear();
        }
    }

    public class BubbleModel : MonoBehaviour
    {
        public event Action<IReadOnlyDictionary<Vector2Int, IBubbleTileModel>> onBubbleTileDictUpdated;
        public event Action<Vector2Int> onBubbleTileAdded;
        public event Action<Vector2Int> onBubbleTileRemoved;

        public IReadOnlyDictionary<Vector2Int, IBubbleTileModel> BubbleTileDict => _bubbleTileDict;
        public HashSet<Vector2Int> RootIndexSet => _rootIndexSet;

        private const int MaxTurns = 1000;

        private int _currentTurn;
        private List<IBubbleTileModel> _flowBubbleList;
        private List<IBubbleTileModel> _newFlowBubbleList;
        private Dictionary<Vector2Int, IBubbleTileModel> _bubbleTileDict;
        private HashSet<Vector2Int> _rootIndexSet;

        private IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> _flowTileListDict;
        private Func<Color> _GetRandomBubbleTileColor;

        private void Awake()
        {
            _currentTurn = 0;
            _flowBubbleList = new();
            _newFlowBubbleList = new();
            _bubbleTileDict = new();
            _rootIndexSet = new();
        }

        internal void FeedBubbleTileList(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> flowTileListDict,
                                         Func<Color> getRandomBubbleTileColor)
        {
            _flowTileListDict = flowTileListDict;
            _GetRandomBubbleTileColor = getRandomBubbleTileColor;

            MakeRootIndexList();
            FeedBubbleTileList();
        }

        private void MakeRootIndexList()
        {
            foreach (LinkedList<IFlowTileModel> flowTileList in _flowTileListDict.Values)
            {
                _rootIndexSet.Add(flowTileList.First.Value.TileIndex);
            }
        }

        public void FeedBubbleTileList()
        {
            ResetTurn();

            int expectedCount = _flowTileListDict.First().Value.Count;
            while (IsAllTileFilled(expectedCount) == false && _currentTurn < MaxTurns)
            {
                ProcessTurn(withNewBubbles: true);
            }

            if (_currentTurn < MaxTurns)
            {
                //ProcessTurn(withNewBubbles: false);
            }
            else
            {
                Debug.LogWarning("최대 턴 수에 도달했습니다.");
            }

            onBubbleTileDictUpdated?.Invoke(_bubbleTileDict);
        }

        private void ResetTurn()
        {
            _currentTurn = 0;
        }

        private void ProcessTurn(bool withNewBubbles)
        {
            _currentTurn += 1;

            _newFlowBubbleList.Clear();
            //Debug.Log("=====");
            //Debug.Log("ProcessTurn 1");
            foreach (BubbleTileModel bubbleTile in _flowBubbleList)
            {
                // A-1. 보유한 LinkedList 에서 다음번 노드가 비어 있으면 Skip
                LinkedListNode<IFlowTileModel> nextFlowTileNode = bubbleTile.NextFlowTileNode;
                if (nextFlowTileNode == null)
                {
                    continue;
                }

                // A-2. 다음번 노드의 타일 인덱스에 이미 버블이 존재하면 Skip
                Vector2Int nextTileIndex = nextFlowTileNode.Value.TileIndex;
                if (ContainsBubbleTile(nextTileIndex))
                {
                    continue;
                }

                Vector2Int originTileIndex = bubbleTile.TileIndex;
                bubbleTile.MoveToNext(_currentTurn);
                Vector2Int movedTileIndex = bubbleTile.TileIndex;
                //Debug.Log($"ProcessTurn 2 : {_currentTurn}, {originTileIndex} -> {movedTileIndex}");
                _bubbleTileDict.Remove(originTileIndex);
                _bubbleTileDict.Add(movedTileIndex, bubbleTile);
            }

            if (withNewBubbles)
            {
                foreach (var pair in _flowTileListDict)
                {
                    LinkedList<IFlowTileModel> tileLinkedList = pair.Value;
                    LinkedListNode<IFlowTileModel> headFlowTileNode = tileLinkedList.First;
                    Vector2Int tileIndex = headFlowTileNode.Value.TileIndex;
                    // A-3. LinkedList 첫번째 노드의 타일 인덱스에 이미 버블이 존재하면 Skip
                    if (ContainsBubbleTile(tileIndex))
                    {
                        continue;
                    }

                    Color tileColor = pair.Key;
                    Vector2 tilePosition = headFlowTileNode.Value.TilePosition;
                    Color bubbleColor = _GetRandomBubbleTileColor();
                    int attackPoint = UnityEngine.Random.Range(0, 3) == 0 ? 5 : 0; // 1/3 확률로 5점 공격 포인트
                    BubbleTileModel bubbleTile = new(tileIndex, tilePosition, tileColor, bubbleColor, attackPoint, _currentTurn, headFlowTileNode);
                    _newFlowBubbleList.Add(bubbleTile);
                    _bubbleTileDict.Add(tileIndex, bubbleTile);
                }

                _flowBubbleList.AddRange(_newFlowBubbleList);
            }
        }

        public bool IsAllTileFilled(int expectedCount)
        {
            bool result = true;
            foreach (var pair in _flowTileListDict)
            {
                Color color = pair.Key;
                int actualCount = _flowBubbleList.Count(bubble => bubble.TileColor == color);

                if (actualCount < expectedCount)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        internal void AddBubbleTile(Vector2Int tileIndex, Color bubbleColor)
        {
            BubbleTileModel bubbleTileModel = new(tileIndex, bubbleColor);
            _bubbleTileDict.Add(tileIndex, bubbleTileModel);
            onBubbleTileAdded?.Invoke(tileIndex);
        }

        internal Color GetBubbleTileColor(Vector2Int tileIndex)
        {
            Color result = Color.clear;
            if (_bubbleTileDict.TryGetValue(tileIndex, out IBubbleTileModel bubbleTileModel))
            {
                result = bubbleTileModel.BubbleColor;
            }
            else
            {
                //Debug.LogWarning($"GetBubbleTileColor : 버블이 존재하지 않는 타일입니다, {tileIndex}");
            }
            return result;
        }

        internal IBubbleTileModel GetBubbleTile(Vector2Int tileIndex)
        {
            if (_bubbleTileDict.TryGetValue(tileIndex, out IBubbleTileModel bubbleTileModel) == false)
            {
                //Debug.LogWarning($"GetBubbleTile : 버블이 존재하지 않는 타일입니다, {tileIndex}");
            }

            return bubbleTileModel;
        }

        internal IBubbleTileModel RemoveBubbleTile(Vector2Int bubbleTileIndex)
        {
            if (_bubbleTileDict.TryGetValue(bubbleTileIndex, out IBubbleTileModel bubbleTile))
            {
                _bubbleTileDict.Remove(bubbleTileIndex);
                _flowBubbleList.Remove(bubbleTile);
            }
            onBubbleTileRemoved?.Invoke(bubbleTileIndex);
            return bubbleTile;
        }

        internal bool ContainsBubbleTile(Vector2Int tileIndex)
        {
            bool result = false;
            if (_bubbleTileDict.TryGetValue(tileIndex, out IBubbleTileModel bubbleTileModel))
            {
                result = true;
            }
            else
            {
                //Debug.LogWarning($"ContainsBubbleTile : 버블이 존재하지 않는 타일입니다, {tileIndex}");
            }
            return result;
        }
    }
}
