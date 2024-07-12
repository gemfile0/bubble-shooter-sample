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
        private LinkedListNode<IFlowTileModel> _currentNode;
        private Color _bubbleColor;

        public BubbleTileModel(Vector2Int tileIndex, Color tileColor, Color bubbleColor, Vector2 tilePosition, int turn, LinkedListNode<IFlowTileModel> currentNode)
        {
            _tileIndex = tileIndex;
            _tileColor = tileColor;
            _bubbleColor = bubbleColor;

            _movementPathNodeList = new() { new BubbleTilePathNode(tileIndex, tilePosition, turn) };
            _currentNode = currentNode;
        }

        public void MoveToNext(int turn)
        {
            _currentNode = _currentNode.Next;
            if (_currentNode != null)
            {
                _tileIndex = _currentNode.Value.TileIndex;
                Vector2 tilePosition = _currentNode.Value.TilePosition;
                _movementPathNodeList.Add(new BubbleTilePathNode(_tileIndex, tilePosition, turn));
            }
        }
    }

    public class BubbleModel : MonoBehaviour
    {
        public event Action<IReadOnlyCollection<Vector2Int>> onBubbleTileSetUpdated;
        public event Action<Vector2Int> onBubbleTileAdded;

        public IEnumerable<IBubbleTileModel> BubbleTileList => _bubbleTileList;

        private const int MaxTurns = 1000;

        private int _currentTurn;
        private List<BubbleTileModel> _bubbleTileList;
        private List<BubbleTileModel> _newBubbleTileList;
        private HashSet<Vector2Int> _bubbleTileSet;

        private IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> _flowTileListDict;
        private Func<Color> _getRandomBubbleTileColor;

        private void Awake()
        {
            _currentTurn = 0;
            _bubbleTileList = new();
            _newBubbleTileList = new();
            _bubbleTileSet = new();
        }

        internal void FillBubbleTileList(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> flowTileListDict,
                                         Func<Color> getRandomBubbleTileColor)
        {
            _flowTileListDict = flowTileListDict;
            _getRandomBubbleTileColor = getRandomBubbleTileColor;

            int expectedCount = flowTileListDict.First().Value.Count - 1;
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
            _bubbleTileSet.Clear();
            foreach (BubbleTileModel bubbleTile in _bubbleTileList)
            {
                _bubbleTileSet.Add(bubbleTile.TileIndex);
            }
            onBubbleTileSetUpdated?.Invoke(_bubbleTileSet);

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
                    LinkedListNode<IFlowTileModel> headNode = tileLinkedList.First;
                    Vector2Int tileIndex = headNode.Value.TileIndex;
                    Vector2 tilePosition = headNode.Value.TilePosition;

                    Color bubbleColor = _getRandomBubbleTileColor();
                    BubbleTileModel bubbleTileModel = new(tileIndex, tileColor, bubbleColor, tilePosition, _currentTurn, headNode);
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

        internal void AddBubbleTile(Vector2Int tileIndex)
        {
            _bubbleTileSet.Add(tileIndex);
            onBubbleTileAdded?.Invoke(tileIndex);
        }
    }
}
