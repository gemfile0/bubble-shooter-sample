using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleTile : MonoBehaviour
    {
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
    }
}
