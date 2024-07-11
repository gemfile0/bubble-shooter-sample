using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class LevelPlayerScene : BaseLevelScene
    {
        [Header("Presenters")]
        [SerializeField] private FlowPresenter _flowPresenter;
        [SerializeField] private BubblePresenter _bubblePresenter;
        [SerializeField] private BubbleShooter _bubbleShooter;

        [Header("Views")]
        [SerializeField] private WallView _wallView;

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

            // 카메라 위치가 설정된 이후에 호출되어야 하는 로직들
            _bubbleShooter.Init();
            _wallView.Init();
        }
    }
}
