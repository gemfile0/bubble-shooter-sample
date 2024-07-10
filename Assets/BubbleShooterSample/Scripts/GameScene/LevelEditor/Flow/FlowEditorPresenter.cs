using BubbleShooterSample.System;
using System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class FlowEditorPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private FlowEditorModel _flowModel;
        [SerializeField] private FlowEditorView _flowView;
        [SerializeField] private FlowEditorToolUI _flowTileEditingUI;

        public event Action<FlowEditorBrushToolType> onFlowEditingToolChanged
        {
            add { _flowTileEditingUI.onFlowEditingToolChanged += value; }
            remove { _flowTileEditingUI.onFlowEditingToolChanged -= value; }
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
            _flowModel.onFlowTileModelDeselected += OnFlowTileModelDeselected;
            _flowModel.onFlowTileModelUpdated += OnFlowTileModelUpdated;
            _flowModel.onFlowTileModelRemoved += OnFlowTileModelRemoved;

            _flowTileEditingUI.onFlowTileTypeChanged += _flowModel.ChangeSelectedFlowTileType;
            _flowTileEditingUI.onColorPicked += _flowView.UpdateFlowTileColor;
        }

        private void OnDisable()
        {
            _flowModel.onFlowTileModelCreated -= OnFlowTileModelCreated;
            _flowModel.onFlowTileModelSelected -= OnFlowTileModelSelected;
            _flowModel.onFlowTileModelDeselected -= OnFlowTileModelDeselected;
            _flowModel.onFlowTileModelUpdated -= OnFlowTileModelUpdated;
            _flowModel.onFlowTileModelRemoved -= OnFlowTileModelRemoved;

            _flowTileEditingUI.onFlowTileTypeChanged -= _flowModel.ChangeSelectedFlowTileType;
            _flowTileEditingUI.onColorPicked -= _flowView.UpdateFlowTileColor;
        }

        private void OnFlowTileModelSelected(IFlowTileModel model)
        {
            _flowTileEditingUI.UpdateTileTypeObject(true);
            _flowTileEditingUI.UpdateTileType(model.TileType);
        }

        private void OnFlowTileModelDeselected()
        {
            _flowTileEditingUI.UpdateTileTypeObject(false);
        }

        private void OnFlowTileModelCreated(IFlowTileModel model)
        {
            _flowView.CreateFlowTile(model.TileIndex, model.TilePosition, model.TileType, model.TileColor);
        }

        private void OnFlowTileModelRemoved(Vector2Int tileIndex)
        {
            _flowView.RemoveFlowTile(tileIndex);
        }

        private void OnFlowTileModelUpdated(IFlowTileModel model)
        {
            _flowView.UpdateFlowTileType(model.TileIndex, model.TilePosition, model.TileType);

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type)
        {
            _flowModel.CreateFlowTile(index, position, type, _flowTileEditingUI.PickedColor);

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

        internal void UpdateToolType(FlowEditorBrushToolType toolType)
        {
            _flowTileEditingUI.UpdateToolType(toolType);
        }
    }
}
