using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GridModel : MonoBehaviour
    {
        public IEnumerable<Vector2Int> GridTileIndexSet => _gridTileIndexSet;
        private HashSet<Vector2Int> _gridTileIndexSet;

        public IEnumerable<Vector2Int> OccupiedTileSet => _occupiedTileSet;
        private HashSet<Vector2Int> _occupiedTileSet;
        private List<Vector2Int> _neighborIndexList;
        private HashSet<Vector2Int> _visitedIndexSet;

        public void Init()
        {
            _gridTileIndexSet = new();
            _neighborIndexList = new();
            _occupiedTileSet = new();
        }

        public void CreateGrid(int rowCount, int columnCount)
        {
            for (int row = 0; row < rowCount; row++)
            {
                // 짝수 행은 한 개 많게    
                int colsInRow = (row % 2 == 0) ? columnCount : columnCount + 1;
                for (int col = 0; col < colsInRow; col++)
                {
                    Vector2Int tileIndex = new Vector2Int(col, row);
                    _gridTileIndexSet.Add(tileIndex);
                }
            }
        }

        internal void ClearOccupiedTiles()
        {
            _occupiedTileSet.Clear();
        }

        public void OccupyTile(Vector2Int tileIndex)
        {
            _occupiedTileSet.Add(tileIndex);
        }

        public void VacateTile(Vector2Int tileIndex)
        {
            _occupiedTileSet.Remove(tileIndex);
        }

        public IEnumerable<Vector2Int> GetNeighborIndexList(Vector2Int tileIndex, HashSet<Vector2Int> visitedIndexSet)
        {
            _visitedIndexSet = visitedIndexSet;

            _neighborIndexList.Clear();
            AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y));  // left
            AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y));  // right
            AddIfExists(new Vector2Int(tileIndex.x, tileIndex.y - 1));  // bottom
            AddIfExists(new Vector2Int(tileIndex.x, tileIndex.y + 1));  // top

            if (tileIndex.y % 2 == 0)
            {
                AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y + 1)); // top-right
                AddIfExists(new Vector2Int(tileIndex.x + 1, tileIndex.y - 1)); // bottom-right
            }
            else
            {
                AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y + 1)); // top-left
                AddIfExists(new Vector2Int(tileIndex.x - 1, tileIndex.y - 1)); // bottom-left
            }

            return _neighborIndexList;
        }

        private void AddIfExists(Vector2Int tileIndex)
        {
            if (_occupiedTileSet.Contains(tileIndex)
                && _visitedIndexSet.Contains(tileIndex) == false
                && _gridTileIndexSet.Contains(tileIndex))
            {
                _neighborIndexList.Add(tileIndex);
            }
        }
    }
}
