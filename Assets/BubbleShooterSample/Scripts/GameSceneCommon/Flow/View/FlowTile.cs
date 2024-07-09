using TMPro;
using UnityEngine;

namespace BubbleShooterSample
{
    public class FlowTile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _typeText;

        public Transform CachedTransform
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

        public FlowTileType Type => _tileType;
        private FlowTileType _tileType;

        public Vector2Int TileIndex => _tileIndex;
        private Vector2Int _tileIndex;

        public void Init(Vector2Int tileIndex, FlowTileType tileType)
        {
            _tileIndex = tileIndex;
            _tileType = tileType;

            UpdateTypeText();
        }

        public void UpdateTileType(FlowTileType tileType)
        {
            _tileType = tileType;

            UpdateTypeText();
        }

        private void UpdateTypeText()
        {
            _typeText.text = _tileType.ToString()[0].ToString();
        }
    }
}
