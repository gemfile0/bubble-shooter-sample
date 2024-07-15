using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class GateKeeperEditorView : MonoBehaviour
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
