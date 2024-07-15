using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class BubbleShooterEditorView : MonoBehaviour
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
