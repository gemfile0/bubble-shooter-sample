using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class BubbleShooterEditorPresenter : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private BubbleShooterData _bubbleShooterData;
        [SerializeField] private BubbleShooterEditorModel _bubbleShooterEditorModel;
        [SerializeField] private BubbleShooterEditorView _bubbleShooterEditorView;

        private Vector3 _totalGridSize;
        private float _horizontalSpacing;

        public void Init(Vector3 totalGridSize, float horizontalSpacing)
        {
            _totalGridSize = totalGridSize;
            _horizontalSpacing = horizontalSpacing;
        }

        protected void UpdateShooterPosition()
        {
            Vector2 gridSizeHalf = _totalGridSize / 2f;
            float horizontalSpacingHalf = _horizontalSpacing / 2f;

            _bubbleShooterEditorView.CachedTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _bubbleShooterData.BubbleShooterPositionY);
        }
    }
}
