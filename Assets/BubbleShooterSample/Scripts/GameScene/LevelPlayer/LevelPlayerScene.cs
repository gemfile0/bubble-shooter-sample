using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class LevelPlayerScene : BaseLevelScene
    {
        [SerializeField] private FlowPresenter _flowPresenter;
        [SerializeField] private BubblePresenter _bubblePresenter;

        [Header("DOTween Settings")]
        [SerializeField] private int _tweenCapacity = 1000;
        [SerializeField] private int _sequenceCapacity = 100;

        private void Awake()
        {
            DOTween.SetTweensCapacity(_tweenCapacity, _sequenceCapacity);
        }

        private void OnEnable()
        {
            _flowPresenter.onColorTileListDictUpdated += _bubblePresenter.UpdateFlowTileListDict;
        }

        private void OnDisable()
        {
            _flowPresenter.onColorTileListDictUpdated -= _bubblePresenter.UpdateFlowTileListDict;
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}
