using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperPresenter : MonoBehaviour,
        ILootAnimationEndPoint
    {
        [SerializeField] private GateKeeperModel _gateKeeperModel;

        [Header("View")]
        [SerializeField] private GateKeeperView _gateKeeperView;
        [SerializeField] private GateKeeperUI _gateKeeperUI;
        [SerializeField] private float healthSliderDuration = .5f;
        [SerializeField] private Ease healthSliderEase = Ease.OutQuad;

        private void OnEnable()
        {
            _gateKeeperModel.onHealthPointRestored += _gateKeeperUI.InitHealthSlider;
            _gateKeeperModel.onHealthPointChanged += OnHealthPointChanged;
        }

        private void OnDisable()
        {
            _gateKeeperModel.onHealthPointRestored -= _gateKeeperUI.InitHealthSlider;
            _gateKeeperModel.onHealthPointChanged -= OnHealthPointChanged;
        }

        public void Init(Vector3 gateKeeperPosition)
        {
            _gateKeeperView.CachedTransform.position = gateKeeperPosition;

            _gateKeeperModel.Init(50);
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
            _gateKeeperUI.UpdateHealthSlider(healthPoint, healthSliderDuration, healthSliderEase);
        }
    }
}
