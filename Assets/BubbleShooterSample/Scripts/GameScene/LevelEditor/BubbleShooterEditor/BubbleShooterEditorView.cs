using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class BubbleShooterEditorView : MonoBehaviour
    {
        [SerializeField] private Transform _shooterTransform;

        public Transform CachedTransform { get; private set; }

        private void Awake()
        {
            CachedTransform = transform;
        }
    }
}
