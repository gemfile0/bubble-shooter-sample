using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class FlowPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private FlowModel _flowModel;
        [SerializeField] private FlowView _flowView;

        public ILevelDataManager LevelDataManager { private get; set; }

        public LevelDataId RestoreLevelID => LevelDataId.Flow;

        private void Awake()
        {
            _flowModel.Init();
        }

        private void OnEnable()
        {
            _flowModel.onFlowTileModelCreated += OnFlowTileModelCreated;
        }

        private void OnDisable()
        {
            _flowModel.onFlowTileModelCreated -= OnFlowTileModelCreated;
        }

        private void OnFlowTileModelCreated(IFlowTileModel model)
        {
            _flowView.CreateFlowTile(model.TilePosition, model.TileType);
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type)
        {
            _flowModel.CreateFlowTile(index, position, type);

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        public string SaveLevelData()
        {
            return _flowModel.SaveLevelData();
        }

        public void RestoreLevelData(string dataStr)
        {
            _flowModel.RestoreLevelData(dataStr);
        }

    }
}
