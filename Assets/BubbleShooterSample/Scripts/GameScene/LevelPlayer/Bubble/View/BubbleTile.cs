using DG.Tweening;
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

        public Sequence Sequence => _sequence;
        private Sequence _sequence;

        private float _moveDuration;

        public void Init(Color bubbleColor, float moveDuration)
        {
            if (_sequence == null)
            {
                _sequence = DOTween.Sequence();
            }
            _spriteRenderer.color = bubbleColor;
            _moveDuration = moveDuration;
        }

        public void Move(Vector2 tilePosition, int turn)
        {
            _sequence.Insert(turn * _moveDuration, CachedTransform.DOMove(tilePosition, _moveDuration).SetEase(Ease.Linear));
        }
    }
}
