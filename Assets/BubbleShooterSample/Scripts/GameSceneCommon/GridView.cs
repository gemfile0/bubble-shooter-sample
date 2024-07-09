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

        private Transform CachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }
                return _cachedTransform;
            }
        }
        private Transform _cachedTransform;

        private List<Vector2> _gridPositions = new List<Vector2>();

        void Start()
        {
            CreateGrid();
        }

        void CreateGrid()
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

                    Vector2 position = new Vector2(xPos, yPos);
                    _gridPositions.Add(position);

                    GridTile grid = Instantiate(_gridPrefab, position, Quaternion.identity, CachedTransform);
                    grid.Init(new Vector2(col, row));
                }
            }
        }

        public List<Vector2> GetGridPositions()
        {
            return _gridPositions;
        }
    }
}