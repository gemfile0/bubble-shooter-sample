using UnityEngine;

namespace BubbleShooterSample
{
    public class LootAnimationSpriteContainer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

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

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private int _itemIndex;

        internal void Init(int itemIndex)
        {
            _itemIndex = itemIndex;
        }

        internal void UpdateSpriteRenderer(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}
