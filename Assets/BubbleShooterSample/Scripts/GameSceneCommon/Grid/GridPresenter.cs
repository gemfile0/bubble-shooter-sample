using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GridPresenter : MonoBehaviour
    {
        [SerializeField] private int _rowCount = 9;
        [SerializeField] private int _columnCount = 10;

        [SerializeField] private GridModel _gridModel;
        [SerializeField] private GridView _gridView;

        public int RowCount => _rowCount;
        public int ColumnCount => _columnCount;
        public float HorizontalSpacing => _gridView.HorizontalSpacing;
        public float VerticalSpacing => _gridView.VerticalSpacing;

        private void Awake()
        {
            _gridModel.Init();
        }

        public void Init()
        {
            _gridModel.CreateGrid(_rowCount, _columnCount);
            foreach (Vector2Int gridTileIndex in _gridModel.GridTileIndexSet)
            {
                _gridView.CreateGridTile(gridTileIndex);
            }
        }

        public Vector3 GetTotalGridSize()
        {
            float width = (_columnCount - 1) * HorizontalSpacing + HorizontalSpacing;
            float height = (_rowCount - 1) * VerticalSpacing + VerticalSpacing;

            return new Vector3(width, height, 0);
        }

        public Vector2 GetClosestTilePosition(Vector2 position)
        {
            float minDistance = float.MaxValue;
            Vector2 closestTilePosition = Vector2.zero;

            foreach (Vector2Int gridTileIndex in _gridModel.GridTileIndexSet)
            {
                if (_gridModel.OccupiedTileSet.Contains(gridTileIndex))
                {
                    continue;
                }

                IGridTile gridTile = _gridView.GetGridTile(gridTileIndex);
                float distance = Vector2.Distance(position, gridTile.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTilePosition = gridTile.Position;
                }
            }

            return closestTilePosition;
        }

        internal void UpdateBubbleTileSet(IReadOnlyCollection<Vector2Int> bubbleTileSet)
        {
            _gridModel.ClearOccupiedTiles();
            foreach (Vector2Int tileIndex in bubbleTileSet)
            {
                _gridModel.OccupyTile(tileIndex);
            }
        }
    }
}
