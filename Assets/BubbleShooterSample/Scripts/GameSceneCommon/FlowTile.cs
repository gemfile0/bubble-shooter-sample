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

        public FlowTileType Type => _type;
        private FlowTileType _type;

        public void Init(FlowTileType type)
        {
            _type = type;

            _typeText.text = type.ToString()[0].ToString();
        }
    }
}
