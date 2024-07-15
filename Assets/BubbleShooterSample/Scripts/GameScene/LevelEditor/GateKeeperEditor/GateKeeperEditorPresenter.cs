using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class GateKeeperEditorPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private GateKeeperData _gateKeeperData;
        [SerializeField] private GateKeeperEditorModel _gateKeeperEditorModel;

        [Header("View")]
        [SerializeField] private GateKeeperEditorView _gateKeeperEditorView;
        [SerializeField] private GateKeeperEditorUI _gateKeeperSkinIndexEditorUI;
        [SerializeField] private GateKeeperEditorUI _gateKeeperHealthPointEditorUI;

        public LevelDataId RestoreLevelID => LevelDataId.GateKeeper;

        public ILevelDataManager LevelDataManager { private get; set; }

        internal void Init(Vector3 gateKeeperPosition)
        {
            _gateKeeperEditorView.CachedTransform.position = gateKeeperPosition;
        }

        private void OnEnable()
        {
            _gateKeeperEditorModel.onLevelDataRestored += OnLevelDataRestored;
            _gateKeeperEditorModel.onHealthPointUpdated += OnHealthPointUpdated;
            _gateKeeperEditorModel.onSkinIndexUpdated += OnSkinIndexUpdated;

            _gateKeeperSkinIndexEditorUI.onButtonClick += OnSkinIndexButtonClick;
            _gateKeeperSkinIndexEditorUI.onValueChanged += OnSkinIndexValueChanged;
            _gateKeeperHealthPointEditorUI.onButtonClick += OnHealthPointButtonClick;
            _gateKeeperHealthPointEditorUI.onValueChanged += OnHealthPointValueChanged;
        }

        private void OnDisable()
        {
            _gateKeeperEditorModel.onLevelDataRestored -= OnLevelDataRestored;
            _gateKeeperEditorModel.onHealthPointUpdated -= OnHealthPointUpdated;
            _gateKeeperEditorModel.onSkinIndexUpdated -= OnSkinIndexUpdated;

            _gateKeeperSkinIndexEditorUI.onButtonClick += OnSkinIndexButtonClick;
            _gateKeeperSkinIndexEditorUI.onValueChanged += OnSkinIndexValueChanged;
            _gateKeeperHealthPointEditorUI.onButtonClick -= OnHealthPointButtonClick;
            _gateKeeperHealthPointEditorUI.onValueChanged -= OnHealthPointValueChanged;
        }

        private void OnLevelDataRestored(int skinIndex, int healthPoint)
        {
            OnSkinIndexUpdated(skinIndex);
            OnHealthPointUpdated(healthPoint);
        }

        private void OnHealthPointUpdated(int healthPoint)
        {
            _gateKeeperHealthPointEditorUI.UpdateUI(healthPoint);
        }

        private void OnSkinIndexUpdated(int skinIndex)
        {
            _gateKeeperSkinIndexEditorUI.UpdateUI(skinIndex);
            Sprite gateKeeperSkin = _gateKeeperData.GetGateKeeperSprite(skinIndex);
            _gateKeeperEditorView.UpdateSkin(gateKeeperSkin);
        }

        private void OnHealthPointButtonClick(int countOffset)
        {
            int nextCount = _gateKeeperEditorModel.HealthPoint + countOffset;
            if (nextCount >= 0 && nextCount <= 100)
            {
                _gateKeeperEditorModel.ChangeHealthPoint(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
        }

        private void OnHealthPointValueChanged(int nextCount)
        {
            if (nextCount >= 0 && nextCount <= 100)
            {
                _gateKeeperEditorModel.ChangeHealthPoint(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
            else
            {
                _gateKeeperHealthPointEditorUI.RevertUI();
            }
        }

        private void OnSkinIndexButtonClick(int countOffset)
        {
            int nextCount = _gateKeeperEditorModel.SkinIndex + countOffset;
            if (nextCount >= 0 && nextCount <= _gateKeeperData.LastSkinIndex)
            {
                _gateKeeperEditorModel.ChangeSkinIndex(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
        }

        private void OnSkinIndexValueChanged(int nextCount)
        {
            if (nextCount >= 0 && nextCount <= _gateKeeperData.LastSkinIndex)
            {
                _gateKeeperEditorModel.ChangeSkinIndex(nextCount);
                LevelDataManager.SaveSpecificLevelData(RestoreLevelID);
            }
            else
            {
                _gateKeeperHealthPointEditorUI.RevertUI();
            }
        }

        public void RestoreLevelData(string dataStr)
        {
            _gateKeeperEditorModel.RestoreLevelData(dataStr);
        }

        public string SaveLevelData()
        {
            return _gateKeeperEditorModel.SaveLevelData();
        }

    }
}
