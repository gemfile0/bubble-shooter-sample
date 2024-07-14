using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;

        public Transform CachedTransform { get; private set; }

        private void Start()
        {
            CachedTransform = transform;
        }
    }
}
