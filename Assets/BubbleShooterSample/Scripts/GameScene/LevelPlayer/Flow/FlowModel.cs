using BubbleShooterSample.LevelEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public enum FlowTileType
    {
        Node = 0,
        Head = 1,
    }

    public interface IFlowTileModel
    {
        public Vector2Int TileIndex { get; }
        public Vector2 TilePosition { get; }
        public FlowTileType TileType { get; }
        public Color TileColor { get; }

        void UpdateTileType(FlowTileType tileType);
    }

    public class FlowTileModel : IFlowTileModel
    {
        public Vector2Int TileIndex => _tileIndex;
        public Vector2 TilePosition => _tilePosition;
        public FlowTileType TileType => _tileType;
        public Color TileColor => _tileColor;

        private Vector2Int _tileIndex;
        private Vector2 _tilePosition;
        private FlowTileType _tileType;
        private Color _tileColor;

        public FlowTileModel(Vector2Int tileIndex, Vector2 tilePosition, FlowTileType tileType, Color tileColor)
        {
            _tileIndex = tileIndex;
            _tilePosition = tilePosition;
            _tileType = tileType;
            _tileColor = tileColor;
        }

        public void UpdateTileType(FlowTileType tileType)
        {
            _tileType = tileType;
        }
    }

    public class FlowModel : MonoBehaviour
    {
        public event Action<IFlowTileModel> onFlowTileModelCreated;
        public event Action<IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>>> onColorTileListDictUpdated;

        private Dictionary<Vector2Int, IFlowTileModel> _flowTileModelDict;

        private Dictionary<Color, LinkedList<IFlowTileModel>> _colorTileListDict;
        private Dictionary<Color, IFlowTileModel> _headTileDict;

        public void Init()
        {
            _flowTileModelDict = new();
            _colorTileListDict = new();
            _headTileDict = new();
        }

        internal void CreateFlowTile(Vector2Int index, Vector2 position, FlowTileType type, Color color)
        {
            IFlowTileModel flowTileModel = new FlowTileModel(index, position, type, color);
            _flowTileModelDict.Add(index, flowTileModel);
            onFlowTileModelCreated?.Invoke(flowTileModel);
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

                SplitAsFlowTileModelList_ByColor();
            }
        }

        private void SplitAsFlowTileModelList_ByColor()
        {
            foreach (IFlowTileModel tileModel in _flowTileModelDict.Values)
            {
                Color tileColor = tileModel.TileColor;
                if (_colorTileListDict.ContainsKey(tileColor) == false)
                {
                    _colorTileListDict.Add(tileColor, new LinkedList<IFlowTileModel>());
                }

                FlowTileType tileType = tileModel.TileType;
                if (tileType == FlowTileType.Head)
                {
                    _headTileDict.Add(tileColor, tileModel);
                }
            }

            foreach (var pair in _headTileDict)
            {
                Color tileColor = pair.Key;
                IFlowTileModel headTile = pair.Value;

                LinkedList<IFlowTileModel> linkedList = _colorTileListDict[tileColor];
                linkedList.AddFirst(headTile);

                IFlowTileModel currentTile = headTile;
                HashSet<Vector2Int> visitedTiles = new() { currentTile.TileIndex };

                while (true)
                {
                    IFlowTileModel nextFlowTile = null;

                    foreach (IFlowTileModel flowTile in _flowTileModelDict.Values)
                    {
                        if (flowTile.TileColor == tileColor
                            && flowTile.TileType == FlowTileType.Node
                            && visitedTiles.Contains(flowTile.TileIndex) == false)
                        {
                            nextFlowTile = flowTile;
                            break;
                        }
                    }

                    if (nextFlowTile == null)
                    {
                        break;
                    }

                    _colorTileListDict[tileColor].AddLast(nextFlowTile);
                    visitedTiles.Add(nextFlowTile.TileIndex);
                    currentTile = nextFlowTile;
                }
            }

            onColorTileListDictUpdated?.Invoke(_colorTileListDict);
        }
    }
}
