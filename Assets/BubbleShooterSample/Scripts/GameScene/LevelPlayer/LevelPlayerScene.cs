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
            _bubblePresenter.onBubbleTileSetUpdated += _gridPresenter.OccupyBubbleTileSet;
            _bubblePresenter.onBubbleTileAdded += _gridPresenter.OccupyBubbleTile;
            _bubblePresenter.onBubbleSequeceComplete += _bubbleShooter.SetReadyToShoot;
            _bubbleShooter.requestGettingClosestTileInfo += _gridPresenter.GetClosestTileInfo;
            _bubbleShooter.requestAddingBubbleTile += _bubblePresenter.AddBubbleTile;
        }

        private void OnDisable()
        {
            _flowPresenter.onColorTileListDictUpdated -= _bubblePresenter.UpdateFlowTileListDict;
            _bubblePresenter.onBubbleTileSetUpdated -= _gridPresenter.OccupyBubbleTileSet;
            _bubblePresenter.onBubbleTileAdded -= _gridPresenter.OccupyBubbleTile;
            _bubblePresenter.onBubbleSequeceComplete -= _bubbleShooter.SetReadyToShoot;
            _bubbleShooter.requestGettingClosestTileInfo -= _gridPresenter.GetClosestTileInfo;
            _bubbleShooter.requestAddingBubbleTile -= _bubblePresenter.AddBubbleTile;
        }

        protected override void Start()
        {
            base.Start();

            // 카메라 위치가 설정된 이후에 호출되어야 하는 로직들
            _bubbleShooter.Init(_gridPresenter.GetTotalGridSize(), _gridPresenter.HorizontalSpacing);
            _wallView.Init();
        }
    }
}
