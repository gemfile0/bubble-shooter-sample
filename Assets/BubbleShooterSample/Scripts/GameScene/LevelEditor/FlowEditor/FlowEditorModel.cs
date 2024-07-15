using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    [Serializable]
    public class FlowTileSaveData
    {
        public Vector2Int Index;
        public Vector2 Position;
        public FlowTileType Type;
        public Color Color;
    }

    [Serializable]
    public class FlowSaveData
    {
        public List<FlowTileSaveData> saveDataList;
    }

    public class FlowEditorModel : MonoBehaviour
    {
        public event Action<IFlowTileModel> onFlowTileModelCreated;
        public event Action<IFlowTileModel> onFlowTileModelSelected;
        public event Action<IFlowTileModel> onFlowTileModelUpdated;
        public event Action<Vector2Int> onFlowTileModelRemoved;
        public event Action onFlowTileModelDeselected;

        private Dictionary<Vector2Int, IFlowTileModel> _flowTileModelDict;
        private IFlowTileModel _selectedFlowTile;

        public void Init()
        {
            _flowTileModelDict = new();
        }

        internal string SaveLevelData()
        {
            FlowSaveData data = new()
            {
                saveDataList = _flowTileModelDict.Values
                    .Select((IFlowTileModel flowTileModel) => new FlowTileSaveData
                    {
                        Index = flowTileModel.TileIndex,
                        Position = flowTileModel.TilePosition,
                        Type = flowTileModel.TileType,
                        Color = flowTileModel.TileColor
                    })
                    .ToList()
            };
            return JsonUtility.ToJson(data);
        }

        internal void RestoreLevelData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                FlowSaveData saveData = JsonUtility.FromJson<FlowSaveData>(dataStr);
                foreach (FlowTileSaveData flowTileSaveData in saveData.saveDataList)
                {
                    //Debug.Log($"RestoreLevelData : {flowTileSaveData.Index}, {flowTileSaveData.Position}, {flowTileSaveData.Type}, {flowTileSaveData.Color}");
                    CreateFlowTile(flowTileSaveData.Index, flowTileSaveData.Position, flowTileSaveData.Type, flowTileSaveData.Color);
                }
            }
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type, Color color)
        {
            IFlowTileModel flowTileModel = new FlowTileModel(index, position, type, color);
            _flowTileModelDict.Add(index, flowTileModel);
            onFlowTileModelCreated?.Invoke(flowTileModel);
        }

        internal bool HasFlowTile(Vector2Int index)
        {
            return _flowTileModelDict.ContainsKey(index);
        }

        internal void SelectFlowTile(Vector2Int index)
        {
            _selectedFlowTile = _flowTileModelDict[index];
            onFlowTileModelSelected?.Invoke(_selectedFlowTile);
        }

        internal void ChangeSelectedFlowTileType(FlowTileType tileType)
        {
            if (_selectedFlowTile != null)
            {
                _selectedFlowTile.UpdateTileType(tileType);
                onFlowTileModelUpdated?.Invoke(_selectedFlowTile);
            }
        }

        internal void RemoveFlowTile(Vector2Int index)
        {
            _flowTileModelDict.Remove(index);
            onFlowTileModelRemoved?.Invoke(index);
        }

        internal void DeselectFlowTile()
        {
            if (_selectedFlowTile != null)
            {
                _selectedFlowTile = null;
                onFlowTileModelDeselected?.Invoke();
            }
        }
    }
}
