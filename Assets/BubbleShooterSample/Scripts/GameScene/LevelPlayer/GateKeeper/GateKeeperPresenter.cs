using DG.Tweening;
using System;
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

        public event Func<Vector2Int, Vector3> requestGridTilePosition;

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

        private void Start()
        {
            Vector3 gridTilePosition = requestGridTilePosition(new Vector2Int(5, 1));
            _gateKeeperView.CachedTransform.position = gridTilePosition;

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
