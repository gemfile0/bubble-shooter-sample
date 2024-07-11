using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider2D;

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

        public void SetColliderEnabled(bool value)
        {
            _collider2D.enabled = value;
        }
    }
}
