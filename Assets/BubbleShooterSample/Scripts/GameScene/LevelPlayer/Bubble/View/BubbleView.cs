using BubbleShooterSample.Helper;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;
        [SerializeField] private float _moveDuration = 1f;

        public float MoveDuration => _moveDuration;

        private Transform _cachedTransform;

        private GameObjectPool<BubbleTile> _bubbleTilePool;

        private void Awake()
        {
            _cachedTransform = transform;

            _bubbleTilePool = new(
                _cachedTransform,
                _bubbleTilePrefab.gameObject,
                defaultCapacity: 50
            );
        }

        internal BubbleTile CreateBubbleTile(Color bubbleColor, Vector2 tilePosition)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.Init(bubbleColor);
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
            bubbleTile.CachedTransform.position = tilePosition;
            return bubbleTile;
        }

        internal void AddBubbleTile(BubbleTile bubbleTile)
        {
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
        }
    }
}
