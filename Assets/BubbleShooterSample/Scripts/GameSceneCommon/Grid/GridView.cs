using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GridTile _gridPrefab;
        [SerializeField] private int _rowCount = 9;
        [SerializeField] private int _columnCount = 10;
        [SerializeField] private float _gridRadius = 0.5f;

        public int RowCount => _rowCount;
        public int ColumnCount => _columnCount;

        public float HorizontalSpacing
        {
            get
            {
                if (_horizontalSpacing == 0)
                {
                    _horizontalSpacing = Mathf.Sqrt(3) * _gridRadius;
                }
                return _horizontalSpacing;
            }
        }
        private float _horizontalSpacing;

        public float VerticalSpacing
        {
            get
            {
                if (_verticalSpacing == 0)
                {
                    _verticalSpacing = 1.5f * _gridRadius;
                }
                return _verticalSpacing;
            }
        }
        private float _verticalSpacing;

        private Transform _cachedTransform;

        public IEnumerable<IGridTile> GridTileList
        {
            get => _gridTileDict.Values;
        }
        private Dictionary<Vector2Int, IGridTile> _gridTileDict;

        private List<Vector2Int> _neighborIndexList;
        private HashSet<Vector2Int> _occupiedTiles;

        private void Awake()
        {
            _cachedTransform = transform;

            _gridTileDict = new();
            _neighborIndexList = new();
            _occupiedTiles = new();

            CreateGrid();
        }

        public Vector3 GetTotalGridSize()
        {
            float width = (_columnCount - 1) * HorizontalSpacing + HorizontalSpacing;
            float height = (_rowCount - 1) * VerticalSpacing + VerticalSpacing;

            return new Vector3(width, height, 0);
        }

        private void CreateGrid()
        {
            float horizontalSpacing = HorizontalSpacing;
            float horizontalSpacingHalf = horizontalSpacing / 2f;
            float verticalSpacing = VerticalSpacing;

            for (int row = 0; row < _rowCount; row++)
            {
                // A-1. 짝수 행은 한 개 적게
                int colsInRow = (row % 2 == 0) ? _columnCount : _columnCount - 1;
                for (int col = 0; col < colsInRow; col++)
                {
                    // A-2. 홀수 행일 경우, x 좌표를 오른쪽으로 반 칸 이동
                    float xOffset = (row % 2 != 0) ? horizontalSpacingHalf : 0;
                    float xPos = col * horizontalSpacing + xOffset;
                    float yPos = row * verticalSpacing;

                    Vector2Int tileIndex = new Vector2Int(col, row);
                    Vector2 tilePosition = new Vector2(xPos, yPos);

                    GridTile gridTile = Instantiate(_gridPrefab, tilePosition, Quaternion.identity, _cachedTransform);
                    gridTile.Init(tileIndex, tilePosition);
                    _gridTileDict.Add(tileIndex, gridTile);
                }
            }
        }

        public IEnumerable<Vector2Int> GetNeighborIndexList(Vector2Int tileIndex)
        {
            _neighborIndexList.Clear();
            AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y));  // left
            AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y));  // right
            AddIfExists(new Vector2Int(tileIndex.x, tileIndex.y - 1));  // bottom
            AddIfExists(new Vector2Int(tileIndex.x, tileIndex.y + 1));  // top

            if (tileIndex.y % 2 == 0)
            {
                AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y - 1)); // bottom-left
                AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y + 1)); // top-left
            }
            else
            {
                AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y - 1)); // bottom-right
                AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y + 1)); // top-right
            }

            return _neighborIndexList;
        }

        private void AddIfExists(Vector2Int tileIndex)
        {
            if (_gridTileDict.ContainsKey(tileIndex)
                && _occupiedTiles.Contains(tileIndex) == false)
            {
                _neighborIndexList.Add(tileIndex);
            }
        }

        public Vector2 GetClosestTilePosition(Vector2 position)
        {
            float minDistance = float.MaxValue;
            Vector2 closestTilePosition = Vector2.zero;

            foreach (var pair in _gridTileDict)
            {
                Vector2Int tileIndex = pair.Key;
                if (_occupiedTiles.Contains(tileIndex))
                {
                    continue;
                }

                IGridTile gridTile = pair.Value;
                float distance = Vector2.Distance(position, gridTile.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTilePosition = gridTile.Position;
                }
            }

            return closestTilePosition;
        }

        public void OccupyTile(Vector2Int tileIndex)
        {
            _occupiedTiles.Add(tileIndex);
        }

        internal void ClearOccupiedTiles()
        {
            _occupiedTiles.Clear();
        }
    }
}