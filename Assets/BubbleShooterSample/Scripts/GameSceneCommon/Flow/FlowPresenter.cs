using BubbleShooterSample.System;
using System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class FlowPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private FlowModel _flowModel;
        [SerializeField] private FlowView _flowView;
        [SerializeField] private FlowTileEditingUI _flowEditingUI;

        public event Action<GridEditingToolType> onFlowEditingToolChanged
        {
            add { _flowEditingUI.onFlowEditingToolChanged += value; }
            remove { _flowEditingUI.onFlowEditingToolChanged -= value; }
        }

        public ILevelDataManager LevelDataManager { private get; set; }

        public LevelDataId RestoreLevelID => LevelDataId.Flow;

        private void Awake()
        {
            _flowModel.Init();
        }

        private void OnEnable()
        {
            _flowModel.onFlowTileModelCreated += OnFlowTileModelCreated;
            _flowModel.onFlowTileModelSelected += OnFlowTileModelSelected;
            _flowModel.onFlowTileModelUpdated += OnFlowTileModelUpdated;
            _flowModel.onFlowTileModelRemoved += OnFlowTileModelRemoved;

            _flowEditingUI.onFlowTileTypeChanged += _flowModel.ChangeSelectedFlowTileType;
        }

        private void OnDisable()
        {
            _flowModel.onFlowTileModelCreated -= OnFlowTileModelCreated;
            _flowModel.onFlowTileModelSelected -= OnFlowTileModelSelected;
            _flowModel.onFlowTileModelUpdated -= OnFlowTileModelUpdated;
            _flowModel.onFlowTileModelRemoved -= OnFlowTileModelRemoved;

            _flowEditingUI.onFlowTileTypeChanged -= _flowModel.ChangeSelectedFlowTileType;
        }

        private void OnFlowTileModelSelected(IFlowTileModel model)
        {
            _flowEditingUI.SetActive();
        }

        private void OnFlowTileModelCreated(IFlowTileModel model)
        {
            _flowView.CreateFlowTile(model.TileIndex, model.TilePosition, model.TileType);
        }

        private void OnFlowTileModelRemoved(Vector2Int tileIndex)
        {
            _flowView.RemoveFlowTile(tileIndex);
        }

        private void OnFlowTileModelUpdated(IFlowTileModel model)
        {
            _flowView.UpdateFlowTile(model.TileIndex, model.TilePosition, model.TileType);

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
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

        internal bool HasFlowTile(Vector2Int index)
        {
            return _flowModel.HasFlowTile(index);
        }

        internal void SelectFlowTile(Vector2Int index)
        {
            _flowModel.SelectFlowTile(index);
        }

        internal void RemoveFlowTile(Vector2Int index)
        {
            _flowModel.RemoveFlowTile(index);
            _flowModel.DeselectFlowTile();

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        internal void UpdateToolType(GridEditingToolType toolType)
        {
            _flowEditingUI.UpdateToolType(toolType);
        }
    }
}
