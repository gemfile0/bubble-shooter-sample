using BubbleShooterSample.System;
using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperPresenter : MonoBehaviour,
        ILootAnimationEndPoint,
        ILevelRestorable
    {
        [SerializeField] private GateKeeperData _gateKeeperData;
        [SerializeField] private GateKeeperModel _gateKeeperModel;

        [Header("View")]
        [SerializeField] private GateKeeperView _gateKeeperView;
        [SerializeField] private GateKeeperUI _gateKeeperUI;

        public LevelDataId RestoreLevelID => LevelDataId.GateKeeper;

        public void Init(Vector3 gateKeeperPosition)
        {
            _gateKeeperView.CachedTransform.position = gateKeeperPosition;
        }

        private void OnEnable()
        {
            _gateKeeperModel.onLevelDataRestored += OnLevelDataRestored;
            _gateKeeperModel.onHealthPointChanged += OnHealthPointChanged;
        }

        private void OnDisable()
        {
            _gateKeeperModel.onLevelDataRestored -= OnLevelDataRestored;
            _gateKeeperModel.onHealthPointChanged -= OnHealthPointChanged;
        }

        private void OnLevelDataRestored(int skinIndex, int healthPoint)
        {
            Sprite gateKeeperSprite = _gateKeeperData.GetGateKeeperSprite(skinIndex);
            _gateKeeperView.UpdateSkin(gateKeeperSprite);
            _gateKeeperUI.UpdateSkin(gateKeeperSprite);
            _gateKeeperUI.InitHealthSlider(healthPoint);
        }

        public void BeginLootAnimation()
        {
            // Do nothing
        }

        public void EndLootAnimation(long attackPoint)
        {
            _gateKeeperModel.Attack((int)attackPoint);
        }

        public Vector3 GetLootAnimtionEndPoint()
        {
            return _gateKeeperView.CachedTransform.position;
        }

        private void OnHealthPointChanged(int healthPoint)
        {
            float healthSliderDuration = _gateKeeperData.HealthSliderDuration;
            Ease healthSliderEase = _gateKeeperData.HealthSliderEase;
            _gateKeeperUI.UpdateHealthSlider(healthPoint, healthSliderDuration, healthSliderEase);
        }

        public void RestoreLevelData(string dataStr)
        {
            _gateKeeperModel.RestoreLevelData(dataStr);
        }
    }
}
