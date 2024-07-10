using UnityEditor;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private BubblePresenter _bubblePresenter;
        [SerializeField] private GridView _gridView;

        [Header("Shooter Transform")]
        [SerializeField] private Transform _shooterTransform;
        [SerializeField] private float _shooterPositionY = -5f;

        private BubbleTile _bubbleTile;

        private void Start()
        {
            Vector2 gridSizeHalf = _gridView.GetTotalGridSize() / 2f;
            float horizontalSpacingHalf = _gridView.HorizontalSpacing / 2f;

            _shooterTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _shooterPositionY);
            _bubbleTile = _bubblePresenter.CreateBubbleTile(_shooterTransform.position);
            _bubbleTile.CachedTransform.position = _shooterTransform.position + new Vector3(0, 1.9f / 2, 0);
            _bubbleTile.SetRendererAlpha(1f);
        }

        private void Update()
        {

        }

        public void OnPingClick()
        {
            EditorGUIUtility.PingObject(_bubbleTile);
        }
    }
}
