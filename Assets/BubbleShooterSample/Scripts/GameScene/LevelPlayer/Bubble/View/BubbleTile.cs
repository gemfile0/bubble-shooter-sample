using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleTile : MonoBehaviour
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

        public void Init(Color bubbleColor)
        {
            bubbleColor.a = 0f;
            _spriteRenderer.color = bubbleColor;
        }

        public void SetRendererAlpha(float value)
        {
            Color bubbleColor = _spriteRenderer.color;
            bubbleColor.a = value;
            _spriteRenderer.color = bubbleColor;
        }
    }
}
