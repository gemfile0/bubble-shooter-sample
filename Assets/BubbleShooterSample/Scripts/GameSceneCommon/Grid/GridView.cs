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

        private List<Vector2Int> neighborIndexList;

        private void Awake()
        {
            _cachedTransform = transform;

            _gridTileDict = new();
            neighborIndexList = new();

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
            neighborIndexList.Clear();
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

            return neighborIndexList;
        }

        private void AddIfExists(Vector2Int tileIndex)
        {
            if (_gridTileDict.ContainsKey(tileIndex))
            {
                neighborIndexList.Add(tileIndex);
            }
        }
    }
}