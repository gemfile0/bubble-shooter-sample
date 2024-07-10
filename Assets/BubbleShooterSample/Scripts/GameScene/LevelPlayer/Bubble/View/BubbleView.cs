using BubbleShooterSample.Helper;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;
        [SerializeField] private float _moveDuration = 1f;

        public float MoveDuration => _moveDuration;

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

        private GameObjectPool<BubbleTile> _bubbleTilePool;

        private void Awake()
        {
            _bubbleTilePool = new(
                CachedTransform,
                _bubbleTilePrefab.gameObject,
                defaultCapacity: 50
            );
        }

        internal BubbleTile CreateBubbleTile(Color bubbleColor, Vector2 tilePosition)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.Init(bubbleColor);
            bubbleTile.CachedTransform.SetParent(CachedTransform);
            bubbleTile.CachedTransform.position = tilePosition;
            return bubbleTile;
        }
    }
}
