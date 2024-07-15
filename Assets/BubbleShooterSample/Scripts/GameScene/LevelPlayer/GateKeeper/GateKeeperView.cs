using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _gateKeeperRenderer;

        public Transform CachedTransform { get; private set; }

        private void Awake()
        {
            CachedTransform = transform;
        }

        internal void UpdateSkin(Sprite skinSprite)
        {
            _gateKeeperRenderer.sprite = skinSprite;
        }
    }
}
