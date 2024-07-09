using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterSample
{
    [Serializable]
    public class FlowTileSaveData
    {
        public Vector2Int Index;
        public Vector2 Position;
        public FlowTileType TileType;
    }

    [Serializable]
    public class FlowSaveData
    {
        public List<FlowTileSaveData> saveDataList;
    }

    public interface IFlowTileModel
    {
        public Vector2Int TileIndex { get; }
        public Vector2 TilePosition { get; }
        public FlowTileType TileType { get; }
    }

    public class FlowTileModel : IFlowTileModel
    {
        public Vector2Int TileIndex => _tileIndex;
        public Vector2 TilePosition => _tilePosition;
        public FlowTileType TileType => _tileType;

        private Vector2Int _tileIndex;
        private Vector2 _tilePosition;
        private FlowTileType _tileType;

        public FlowTileModel(Vector2Int tileIndex, Vector2 tilePosition, FlowTileType tileType)
        {
            _tileIndex = tileIndex;
            _tilePosition = tilePosition;
            _tileType = tileType;
        }
    }

    public class FlowModel : MonoBehaviour
    {
        public event Action<IFlowTileModel> onFlowTileModelCreated;

        private Dictionary<Vector2Int, IFlowTileModel> _flowTileModelDict;

        public void Init()
        {
            _flowTileModelDict = new();
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type)
        {
            IFlowTileModel flowTileModel = new FlowTileModel(index, position, type);
            _flowTileModelDict.Add(index, flowTileModel);
            onFlowTileModelCreated?.Invoke(flowTileModel);
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
                        TileType = flowTileModel.TileType
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
                    CreateFlowTile(flowTileSaveData.Index, flowTileSaveData.Position, flowTileSaveData.TileType);
                }
            }
        }
    }
}
