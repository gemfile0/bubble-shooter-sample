using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class LevelEditorScene : BaseLevelScene
    {
        [Header("Presenter")]
        [SerializeField] private BubbleShooterEditorPresenter _bubbleShooterEditorPresenter;
        [SerializeField] private GateKeeperEditorPresenter _gateKeeperEditorPresenter;

        protected override void Start()
        {
            base.Start();

            _bubbleShooterEditorPresenter.Init(_totalGridSize, _gridPresenter.HorizontalSpacing);
            _gateKeeperEditorPresenter.Init(_gridPresenter.GetGridTilePosition(new Vector2Int(5, 1)));
        }
    }
}
