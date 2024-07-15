using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class BubbleShooterEditorPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private BubbleShooterData _bubbleShooterData;
        [SerializeField] private BubbleShooterEditorModel _bubbleShooterEditorModel;

        [Header("View")]
        [SerializeField] private BubbleShooterEditorView _bubbleShooterEditorView;
        [SerializeField] private BubbleShooterEditorUI _bubbleShooterEditorUI;

        private Vector3 _totalGridSize;
        private float _horizontalSpacing;

        public ILevelDataManager LevelDataManager { private get; set; }

        public LevelDataId RestoreLevelID => LevelDataId.BubbleShooter;

        public void Init(Vector3 totalGridSize, float horizontalSpacing)
        {
            _totalGridSize = totalGridSize;
            _horizontalSpacing = horizontalSpacing;

            UpdateShooterPosition();
        }

        private void OnEnable()
        {
            _bubbleShooterEditorModel.onBubbleCountRestored += _bubbleShooterEditorUI.UpdateUI;
            _bubbleShooterEditorModel.onBubbleCountUpdated += _bubbleShooterEditorUI.UpdateUI;

            _bubbleShooterEditorUI.onButtonClick += OnButtonClick;
            _bubbleShooterEditorUI.onValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _bubbleShooterEditorModel.onBubbleCountRestored -= _bubbleShooterEditorUI.UpdateUI;
            _bubbleShooterEditorModel.onBubbleCountUpdated -= _bubbleShooterEditorUI.UpdateUI;

            _bubbleShooterEditorUI.onButtonClick -= OnButtonClick;
            _bubbleShooterEditorUI.onValueChanged -= OnValueChanged;
        }

        private void OnButtonClick(int countOffset)
        {
            int nextCount = _bubbleShooterEditorModel.BubbleCount + countOffset;
            if (nextCount >= 0 && nextCount <= 100)
            {
                _bubbleShooterEditorModel.ChangeBubbleCount(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
        }

        private void OnValueChanged(int nextCount)
        {
            if (nextCount >= 0 && nextCount <= 100)
            {
                _bubbleShooterEditorModel.ChangeBubbleCount(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
            else
            {
                _bubbleShooterEditorUI.RevertUI();
            }
        }

        public void RestoreLevelData(string dataStr)
        {
            _bubbleShooterEditorModel.RestoreLevelData(dataStr);
        }

        public string SaveLevelData()
        {
            return _bubbleShooterEditorModel.SaveLevelData();
        }

        protected void UpdateShooterPosition()
        {
            Vector2 gridSizeHalf = _totalGridSize / 2f;
            float horizontalSpacingHalf = _horizontalSpacing / 2f;

            _bubbleShooterEditorView.CachedTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _bubbleShooterData.BubbleShooterPositionY);
        }
    }
}
