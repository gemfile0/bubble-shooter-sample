using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public Transform CachedTransform { get; private set; }

        private void Awake()
        {
            CachedTransform = transform;
        }
    }
}
