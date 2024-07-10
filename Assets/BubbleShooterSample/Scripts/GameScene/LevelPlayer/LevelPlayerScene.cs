using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class LevelPlayerScene : BaseLevelScene
    {
        [SerializeField] private FlowPresenter _flowPresenter;
        [SerializeField] private BubblePresenter _bubblePresenter;

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
