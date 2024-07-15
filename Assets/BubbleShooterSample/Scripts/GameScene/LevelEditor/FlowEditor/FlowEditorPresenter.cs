using BubbleShooterSample.System;
using System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class FlowEditorPresenter : MonoBehaviour,
        ILevelDataManagerSetter,
        ILevelSavable
    {
        [SerializeField] private FlowEditorModel _flowEditorModel;
        [SerializeField] private FlowEditorView _flowEditorView;
        [SerializeField] private FlowEditorToolUI _flowEditorToolUI;

        public event Action<FlowEditorBrushToolType> onFlowEditingToolChanged
        {
            add { _flowEditorToolUI.onFlowEditingToolChanged += value; }
            remove { _flowEditorToolUI.onFlowEditingToolChanged -= value; }
        }

        public ILevelDataManager LevelDataManager { private get; set; }

        public LevelDataId RestoreLevelID => LevelDataId.Flow;

        private void Awake()
        {
            _flowEditorModel.Init();
        }

        private void OnEnable()
        {
            _flowEditorModel.onFlowTileModelCreated += OnFlowTileModelCreated;
            _flowEditorModel.onFlowTileModelSelected += OnFlowTileModelSelected;
            _flowEditorModel.onFlowTileModelDeselected += OnFlowTileModelDeselected;
            _flowEditorModel.onFlowTileModelUpdated += OnFlowTileModelUpdated;
            _flowEditorModel.onFlowTileModelRemoved += OnFlowTileModelRemoved;

            _flowEditorToolUI.onFlowTileTypeChanged += _flowEditorModel.ChangeSelectedFlowTileType;
            _flowEditorToolUI.onColorPicked += _flowEditorView.UpdateFlowTileColor;
        }

        private void OnDisable()
        {
            _flowEditorModel.onFlowTileModelCreated -= OnFlowTileModelCreated;
            _flowEditorModel.onFlowTileModelSelected -= OnFlowTileModelSelected;
            _flowEditorModel.onFlowTileModelDeselected -= OnFlowTileModelDeselected;
            _flowEditorModel.onFlowTileModelUpdated -= OnFlowTileModelUpdated;
            _flowEditorModel.onFlowTileModelRemoved -= OnFlowTileModelRemoved;

            _flowEditorToolUI.onFlowTileTypeChanged -= _flowEditorModel.ChangeSelectedFlowTileType;
            _flowEditorToolUI.onColorPicked -= _flowEditorView.UpdateFlowTileColor;
        }

        private void OnFlowTileModelSelected(IFlowTileModel model)
        {
            _flowEditorToolUI.UpdateTileTypeObject(true);
            _flowEditorToolUI.UpdateTileType(model.TileType);
        }

        private void OnFlowTileModelDeselected()
        {
            _flowEditorToolUI.UpdateTileTypeObject(false);
        }

        private void OnFlowTileModelCreated(IFlowTileModel model)
        {
            _flowEditorView.CreateFlowTile(model.TileIndex, model.TilePosition, model.TileType, model.TileColor);
        }

        private void OnFlowTileModelRemoved(Vector2Int tileIndex)
        {
            _flowEditorView.RemoveFlowTile(tileIndex);
        }

        private void OnFlowTileModelUpdated(IFlowTileModel model)
        {
            _flowEditorView.UpdateFlowTileType(model.TileIndex, model.TilePosition, model.TileType);

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type)
        {
            _flowEditorModel.CreateFlowTile(index, position, type, _flowEditorToolUI.PickedColor);

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        public string SaveLevelData()
        {
            return _flowEditorModel.SaveLevelData();
        }

        public void RestoreLevelData(string dataStr)
        {
            _flowEditorModel.RestoreLevelData(dataStr);
        }

        internal bool HasFlowTile(Vector2Int index)
        {
            return _flowEditorModel.HasFlowTile(index);
        }

        internal void SelectFlowTile(Vector2Int index)
        {
            _flowEditorModel.SelectFlowTile(index);
        }

        internal void RemoveFlowTile(Vector2Int index)
        {
            _flowEditorModel.RemoveFlowTile(index);
            _flowEditorModel.DeselectFlowTile();

            LevelDataManager.SaveSpecificLevelData(LevelDataId.Flow);
        }

        internal void UpdateToolType(FlowEditorBrushToolType toolType)
        {
            _flowEditorToolUI.UpdateToolType(toolType);
        }
    }
}
