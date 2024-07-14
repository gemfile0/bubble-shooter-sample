using TMPro;
using UnityEngine;

namespace BubbleShooterSample
{
    public class LootAnimationTextContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textRenderer;

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

        public TextMeshPro TextMeshPro => _textRenderer;

        internal void UpdateText(string text)
        {
            _textRenderer.text = text;
        }
    }
}
