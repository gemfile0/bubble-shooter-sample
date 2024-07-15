using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class LevelPlayerScene : BaseLevelScene
    {
        [Header("Presenter")]
        [SerializeField] private FlowPresenter _flowPresenter;
        [SerializeField] private BubblePresenter _bubblePresenter;
        [SerializeField] private BubbleShooterPresenter _bubbleShooterPresenter;
        [SerializeField] private LootAnimationController _lootAnimationController;
        [SerializeField] private GateKeeperPresenter _gateKeeperPresenter;

        [Header("View")]
        [SerializeField] private WallView _wallView;

        [Header("DOTween Setting")]
        [SerializeField] private int _tweenCapacity = 1000;
        [SerializeField] private int _sequenceCapacity = 500;

        private void Awake()
        {
            DOTween.SetTweensCapacity(_tweenCapacity, _sequenceCapacity);
        }

        private void OnEnable()
        {
            _flowPresenter.onColorTileListDictUpdated += _bubblePresenter.UpdateFlowTileListDict;

            _bubblePresenter.onBubbleTileDictUpdated += _gridPresenter.OccupyTileSet;
            _bubblePresenter.onBubbleTileAdded += _gridPresenter.OccupyTile;
            _bubblePresenter.onBubbleTileRemoved += _gridPresenter.VacateTile;
            _bubblePresenter.onBubbleSequeceComplete += _bubbleShooterPresenter.SetReadyToShoot;
            _bubblePresenter.requestGettingNeighborIndexList += _gridPresenter.GetNeighborIndexList;

            _bubbleShooterPresenter.requestGettingClosestTileInfo += _gridPresenter.GetClosestTileInfo;
            _bubbleShooterPresenter.requestCreatingBubbleTile += _bubblePresenter.CreateBubbleTile;
            _bubbleShooterPresenter.requestAddingBubbleTile += _bubblePresenter.AddBubbleTile;
        }

        private void OnDisable()
        {
            _flowPresenter.onColorTileListDictUpdated -= _bubblePresenter.UpdateFlowTileListDict;

            _bubblePresenter.onBubbleTileDictUpdated -= _gridPresenter.OccupyTileSet;
            _bubblePresenter.onBubbleTileAdded -= _gridPresenter.OccupyTile;
            _bubblePresenter.onBubbleTileRemoved -= _gridPresenter.VacateTile;
            _bubblePresenter.onBubbleSequeceComplete -= _bubbleShooterPresenter.SetReadyToShoot;
            _bubblePresenter.requestGettingNeighborIndexList -= _gridPresenter.GetNeighborIndexList;

            _bubbleShooterPresenter.requestGettingClosestTileInfo -= _gridPresenter.GetClosestTileInfo;
            _bubbleShooterPresenter.requestCreatingBubbleTile -= _bubblePresenter.CreateBubbleTile;
            _bubbleShooterPresenter.requestAddingBubbleTile -= _bubblePresenter.AddBubbleTile;
        }

        protected override void Start()
        {
            _lootAnimationController.AddLootAnimationEndPoint(LootAnimationType.AttackPoint, _gateKeeperPresenter);

            base.Start();

            _wallView.Init(_totalGridSize); /* 카메라 위치가 설정된 이후에 호출되어야 함 */

            _bubbleShooterPresenter.Init(_totalGridSize, _gridPresenter.HorizontalSpacing);
            _gateKeeperPresenter.Init(_gridPresenter.GetGridTilePosition(new Vector2Int(5, 1)));
        }
    }
}
