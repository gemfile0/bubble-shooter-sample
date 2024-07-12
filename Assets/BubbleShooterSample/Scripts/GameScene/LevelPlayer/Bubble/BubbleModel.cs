using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public struct BubbleTilePathNode
    {
        public Vector2Int TileIndex;
        public Vector3 TilePosition;
        public int Turn;

        public BubbleTilePathNode(Vector2Int tileIndex, Vector3 tilePosition, int turn)
        {
            TileIndex = tileIndex;
            TilePosition = tilePosition;
            Turn = turn;
        }
    }

    public interface IBubbleTileModel
    {
        public Color BubbleColor { get; }
        public BubbleTilePathNode HeadPathNode { get; }
        public IEnumerable<BubbleTilePathNode> MovementPathNodeList { get; }
        public Vector2Int TileIndex { get; }
    }

    public class BubbleTileModel : IBubbleTileModel
    {
        public Color BubbleColor => _bubbleColor;
        public BubbleTilePathNode HeadPathNode => _movementPathNodeList[0];
        public IEnumerable<BubbleTilePathNode> MovementPathNodeList => _movementPathNodeList;
        public Vector2Int TileIndex => _tileIndex;

        public Color TileColor => _tileColor;

        private Vector2Int _tileIndex;
        private Color _tileColor;
        private List<BubbleTilePathNode> _movementPathNodeList;
        private LinkedListNode<IFlowTileModel> _flowTileNode;
        private Color _bubbleColor;

        public BubbleTileModel(Vector2Int tileIndex, Color bubbleColor)
        {
            _tileIndex = tileIndex;
            _bubbleColor = bubbleColor;
        }

        public BubbleTileModel(Vector2Int tileIndex, Color tileColor, Color bubbleColor, Vector2 tilePosition, int turn, LinkedListNode<IFlowTileModel> flowTileNode)
        {
            _tileIndex = tileIndex;
            _tileColor = tileColor;
            _bubbleColor = bubbleColor;

            _movementPathNodeList = new() { new BubbleTilePathNode(tileIndex, tilePosition, turn) };
            _flowTileNode = flowTileNode;
        }

        public void MoveToNext(int turn)
        {
            _flowTileNode = _flowTileNode.Next;
            if (_flowTileNode != null)
            {
                _tileIndex = _flowTileNode.Value.TileIndex;
                Vector2 tilePosition = _flowTileNode.Value.TilePosition;
                _movementPathNodeList.Add(new BubbleTilePathNode(_tileIndex, tilePosition, turn));
            }
        }
    }

    public class BubbleModel : MonoBehaviour
    {
        public event Action<IReadOnlyDictionary<Vector2Int, IBubbleTileModel>> onBubbleTileDictUpdated;
        public event Action<Vector2Int> onBubbleTileAdded;
        public event Action<Vector2Int> onBubbleTileRemoved;

        public IReadOnlyCollection<IBubbleTileModel> BubbleTileList => _bubbleTileList;
        public HashSet<Vector2Int> RootIndexSet => _rootIndexSet;

        private const int MaxTurns = 1000;

        private int _currentTurn;
        private List<BubbleTileModel> _bubbleTileList;
        private List<BubbleTileModel> _newBubbleTileList;
        private Dictionary<Vector2Int, IBubbleTileModel> _bubbleTileDict;
        private HashSet<Vector2Int> _rootIndexSet;

        private IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> _flowTileListDict;
        private Func<Color> _getRandomBubbleTileColor;

        private void Awake()
        {
            _currentTurn = 0;
            _bubbleTileList = new();
            _newBubbleTileList = new();
            _bubbleTileDict = new();
            _rootIndexSet = new();
        }

        internal void FillBubbleTileList(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> flowTileListDict,
                                         Func<Color> getRandomBubbleTileColor)
        {
            _flowTileListDict = flowTileListDict;
            _getRandomBubbleTileColor = getRandomBubbleTileColor;

            MakeRootIndexList();
            FillBubbleTileList();
        }

        private void MakeRootIndexList()
        {
            foreach (LinkedList<IFlowTileModel> flowTileList in _flowTileListDict.Values)
            {
                _rootIndexSet.Add(flowTileList.First.Value.TileIndex);
            }
        }

        private void FillBubbleTileList()
        {
            int expectedCount = _flowTileListDict.First().Value.Count - 1;
            while (IsAllTileFilled(expectedCount) == false && _currentTurn < MaxTurns)
            {
                ProcessTurn(withNewBubbles: true);
            }

            if (_currentTurn < MaxTurns)
            {
                ProcessTurn(withNewBubbles: false);
            }
            else
            {
                Debug.LogWarning("최대 턴 수에 도달했습니다.");
            }

            // 모든 이동이 완료된 후 포지션 등록
            _bubbleTileDict.Clear();
            foreach (BubbleTileModel bubbleTile in _bubbleTileList)
            {
                _bubbleTileDict.Add(bubbleTile.TileIndex, bubbleTile);
            }
            onBubbleTileDictUpdated?.Invoke(_bubbleTileDict);

            //첫 번째 BubbleTileModel의 이동 경로를 로그로 출력
            //if (_bubbleTileList.Count > 0)
            //{
            //    BubbleTileModel firstBubble = _bubbleTileList[0];
            //    Debug.Log($"First Bubble (Color: {firstBubble.TileColor}) Movement Path:");
            //    foreach (BubbleTilePathNode pathNode in firstBubble.MovementPathNodeList)
            //    {
            //        Debug.Log($"Turn {pathNode.Turn}: {pathNode.TileIndex}");
            //    }
            //}
        }

        private void ProcessTurn(bool withNewBubbles)
        {
            _currentTurn += 1;

            _newBubbleTileList.Clear();

            foreach (BubbleTileModel bubbleTile in _bubbleTileList)
            {
                bubbleTile.MoveToNext(_currentTurn);
            }

            if (withNewBubbles)
            {
                foreach (var pair in _flowTileListDict)
                {
                    Color tileColor = pair.Key;
                    LinkedList<IFlowTileModel> tileLinkedList = pair.Value;
                    LinkedListNode<IFlowTileModel> headFlowTileNode = tileLinkedList.First;
                    Vector2Int tileIndex = headFlowTileNode.Value.TileIndex;
                    Vector2 tilePosition = headFlowTileNode.Value.TilePosition;

                    Color bubbleColor = _getRandomBubbleTileColor();
                    BubbleTileModel bubbleTileModel = new(tileIndex, tileColor, bubbleColor, tilePosition, _currentTurn, headFlowTileNode);
                    _newBubbleTileList.Add(bubbleTileModel);
                }

                _bubbleTileList.AddRange(_newBubbleTileList);
            }
        }

        public bool IsAllTileFilled(int expectedCount)
        {
            bool result = true;
            foreach (var pair in _flowTileListDict)
            {
                Color color = pair.Key;
                int actualCount = _bubbleTileList.Count(bubble => bubble.TileColor == color);

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
            _bubbleTileList.Add(bubbleTileModel);
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
                Debug.LogWarning($"GetBubbleTileColor : 버블이 존재하지 않는 타일입니다, {tileIndex}");
            }
            return result;
        }

        internal void RemoveBubbleTile(Vector2Int bubbleTileIndex)
        {
            _bubbleTileDict.Remove(bubbleTileIndex);
            onBubbleTileRemoved?.Invoke(bubbleTileIndex);
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
                Debug.LogWarning($"ContainsBubbleTile : 버블이 존재하지 않는 타일입니다, {tileIndex}");
            }
            return result;
        }
    }
}
