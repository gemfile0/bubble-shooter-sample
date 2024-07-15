using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class LevelEditorScene : BaseLevelScene
    {
        [Header("Presenter")]
        [SerializeField] private BubbleShooterEditorPresenter _bubbleShooterEditorPresenter;

        protected override void Start()
        {
            base.Start();

            _bubbleShooterEditorPresenter.Init(_gridPresenter.GetTotalGridSize(), _gridPresenter.HorizontalSpacing);
        }
    }
}
