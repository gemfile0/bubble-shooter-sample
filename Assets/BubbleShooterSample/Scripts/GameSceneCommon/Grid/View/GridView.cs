using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GridTile _gridPrefab;
        [SerializeField] private float _gridRadius = 0.5f;

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

        internal void Init()
        {
            _cachedTransform = transform;

            _gridTileDict = new();
        }

        internal void CreateGridTile(Vector2Int tileIndex, int rowCount)
        {
            int col = tileIndex.x;
            int row = tileIndex.y;

            // 홀수 행일 경우, x 좌표를 왼쪽으로 반 칸 이동
            float xOffset = (row % 2 != 0) ? -HorizontalSpacing / 2f : 0;
            float xPos = col * HorizontalSpacing + xOffset;

            // 그리드의 전체 높이를 계산하여 yPos를 반대로 배치
            float totalGridHeight = (rowCount - 1) * VerticalSpacing;
            float yPos = totalGridHeight - row * VerticalSpacing;

            Vector2 tilePosition = new Vector2(xPos, yPos);

            GridTile gridTile = Instantiate(_gridPrefab, tilePosition, Quaternion.identity, _cachedTransform);
            gridTile.Init(tileIndex, tilePosition);
            _gridTileDict.Add(tileIndex, gridTile);
        }

        internal IGridTile GetGridTile(Vector2Int gridTileIndex)
        {
            _gridTileDict.TryGetValue(gridTileIndex, out IGridTile gridTile);
            return gridTile;
        }
    }
}