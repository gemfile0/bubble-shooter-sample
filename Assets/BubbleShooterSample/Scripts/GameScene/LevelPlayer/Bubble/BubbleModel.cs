using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public interface IBubbleTileModel
    {
        public BubbleTilePathNode HeadPathNode { get; }
        public IEnumerable<BubbleTilePathNode> MovementPathNodeList { get; }
    }

    public struct BubbleTilePathNode
    {
        public Vector2 tilePosition;
        public int turn;

        public BubbleTilePathNode(Vector2 tilePosition, int turn)
        {
            this.tilePosition = tilePosition;
            this.turn = turn;
        }
    }

    public class BubbleTileModel : IBubbleTileModel
    {
        public BubbleTilePathNode HeadPathNode => _movementPathNodeList[0];
        public IEnumerable<BubbleTilePathNode> MovementPathNodeList => _movementPathNodeList;
        public Vector2Int TileIndex => _tileIndex;
        public Color TileColor => _tileColor;

        private Vector2Int _tileIndex;
        private Color _tileColor;
        private List<BubbleTilePathNode> _movementPathNodeList;
        private LinkedListNode<IFlowTileModel> _currentNode;

        public BubbleTileModel(Vector2Int tileIndex, Vector2 tilePosition, Color tileColor, int turn, LinkedListNode<IFlowTileModel> currentNode)
        {
            _tileIndex = tileIndex;
            _tileColor = tileColor;
            _movementPathNodeList = new() { new BubbleTilePathNode(tilePosition, turn) };
            _currentNode = currentNode;
        }

        public void MoveToNext(int turn)
        {
            _currentNode = _currentNode.Next;
            if (_currentNode != null)
            {
                _tileIndex = _currentNode.Value.TileIndex;
                Vector2 tilePosition = _currentNode.Value.TilePosition;
                _movementPathNodeList.Add(new BubbleTilePathNode(tilePosition, turn));
            }
        }
    }

    public class BubbleModel : MonoBehaviour
    {
        public IEnumerable<IBubbleTileModel> BubbleTileList => _bubbleTileList;

        private const int MaxTurns = 100;

        private int _currentTurn;

        private IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> _colorTileListDict;
        private List<BubbleTileModel> _bubbleTileList;
        private List<BubbleTileModel> _newBubbleTileList;

        private void Awake()
        {
            _currentTurn = 0;
            _bubbleTileList = new();
            _newBubbleTileList = new();
        }

        internal void FillBubbleTileList(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _colorTileListDict = colorTileListDict;

            while (IsAllTileFilled() == false && _currentTurn < MaxTurns)
            {
                ProcessTurn();
            }

            if (_currentTurn >= MaxTurns)
            {
                Debug.LogWarning("최대 턴 수에 도달했습니다.");
            }

            // 첫 번째 BubbleTileModel의 이동 경로를 로그로 출력
            //if (_bubbleTileList.Count > 0)
            //{
            //    var firstBubble = _bubbleTileList[0];
            //    Debug.Log($"First Bubble (Color: {firstBubble.TileColor}) Movement Path:");
            //    foreach (var (position, turn) in firstBubble.MovementPath)
            //    {
            //        Debug.Log($"Turn {turn}: {position}");
            //    }
            //}
        }

        private void ProcessTurn()
        {
            _currentTurn += 1;

            _newBubbleTileList.Clear();

            foreach (BubbleTileModel bubbleTile in _bubbleTileList)
            {
                bubbleTile.MoveToNext(_currentTurn);
            }

            foreach (var pair in _colorTileListDict)
            {
                Color tileColor = pair.Key;
                LinkedList<IFlowTileModel> tileList = pair.Value;
                LinkedListNode<IFlowTileModel> headNode = tileList.First;
                Vector2Int tileIndex = headNode.Value.TileIndex;
                Vector2 tilePosition = headNode.Value.TilePosition;

                _newBubbleTileList.Add(new BubbleTileModel(tileIndex, tilePosition, tileColor, _currentTurn, headNode));
            }

            _bubbleTileList.AddRange(_newBubbleTileList);
        }

        public bool IsAllTileFilled()
        {
            bool result = true;
            foreach (var pair in _colorTileListDict)
            {
                Color color = pair.Key;
                LinkedList<IFlowTileModel> colorTileList = pair.Value;

                foreach (IFlowTileModel colorTile in colorTileList)
                {
                    if (_bubbleTileList.Exists(bubble => bubble.TileIndex == colorTile.TileIndex
                                                         && bubble.TileColor == color) == false)
                    {
                        result = false;
                        break;
                    }
                }

                if (result == false)
                {
                    break;
                }
            }
            return result;
        }
    }
}
