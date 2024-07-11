using UnityEngine;

namespace BubbleShooterSample
{
    public enum FlowTileType
    {
        Node = 0,
        Head = 1,
    }

    public interface IFlowTileModel
    {
        public Vector2Int TileIndex { get; }
        public Vector2 TilePosition { get; }
        public FlowTileType TileType { get; }
        public Color TileColor { get; }

        void UpdateTileType(FlowTileType tileType);
    }

    public class FlowTileModel : IFlowTileModel
    {
        public Vector2Int TileIndex => _tileIndex;
        public Vector2 TilePosition => _tilePosition;
        public FlowTileType TileType => _tileType;
        public Color TileColor => _tileColor;

        private Vector2Int _tileIndex;
        private Vector2 _tilePosition;
        private FlowTileType _tileType;
        private Color _tileColor;

        public FlowTileModel(Vector2Int tileIndex, Vector2 tilePosition, FlowTileType tileType, Color tileColor)
        {
            _tileIndex = tileIndex;
            _tilePosition = tilePosition;
            _tileType = tileType;
            _tileColor = tileColor;
        }

        public void UpdateTileType(FlowTileType tileType)
        {
            _tileType = tileType;
        }
    }
}
