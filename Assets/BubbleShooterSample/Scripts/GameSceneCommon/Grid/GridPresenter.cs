using BubbleShooterSample.LevelPlayer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample
{
    public struct ClosestTileInfo
    {
        public Vector2 Position;
        public Vector2Int Index;
    }

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
            _gridView.Init();
        }

        public void Init()
        {
            _gridModel.CreateGrid(_rowCount, _columnCount);
            foreach (Vector2Int gridTileIndex in _gridModel.GridTileIndexSet)
            {
                _gridView.CreateGridTile(gridTileIndex, _rowCount);
            }
        }

        public Vector3 GetTotalGridSize()
        {
            float width = (_columnCount - 1) * HorizontalSpacing + HorizontalSpacing;
            float height = (_rowCount - 1) * VerticalSpacing + VerticalSpacing;

            return new Vector3(width, height, 0);
        }

        public ClosestTileInfo GetClosestTileInfo(Vector2 position)
        {
            float minDistance = float.MaxValue;
            ClosestTileInfo closestTileInfo = new ClosestTileInfo();

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
                    closestTileInfo.Index = gridTileIndex;
                    closestTileInfo.Position = gridTile.Position;
                }
            }

            return closestTileInfo;
        }

        internal void OccupyTileSet(IReadOnlyDictionary<Vector2Int, IBubbleTileModel> bubbleTileDict)
        {
            _gridModel.ClearOccupiedTiles();
            foreach (var pair in bubbleTileDict)
            {
                Vector2Int tileIndex = pair.Key;
                _gridModel.OccupyTile(tileIndex);
            }
        }

        internal void OccupyTile(Vector2Int tileIndex)
        {
            _gridModel.OccupyTile(tileIndex);
        }

        internal void VacateTile(Vector2Int tileIndex)
        {
            _gridModel.VacateTile(tileIndex);
        }

        public IEnumerable<Vector2Int> GetNeighborIndexList(Vector2Int tileIndex, HashSet<Vector2Int> visitedIndexSet)
        {
            return _gridModel.GetNeighborIndexList(tileIndex, visitedIndexSet);
        }
    }
}
