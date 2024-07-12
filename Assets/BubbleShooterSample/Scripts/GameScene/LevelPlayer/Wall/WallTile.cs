using UnityEngine;

namespace BubbleShooterSample
{
    public enum WallTileType
    {
        Left,
        Right
    }

    public class WallTile : MonoBehaviour
    {
        [SerializeField] private WallTileType _type;

        private Transform _cachedTransform;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        public float GetNearbyX(float offset)
        {
            float result = 0f;
            float widthHalf = _cachedTransform.localScale.x / 2f;
            if (_type == WallTileType.Left)
            {
                result = _cachedTransform.position.x + widthHalf + offset;
            }
            else
            {
                result = _cachedTransform.position.x - widthHalf - offset;
            }
            return result;
        }
    }
}
