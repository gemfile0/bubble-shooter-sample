using BubbleShooterSample.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class FlowEditorView : MonoBehaviour
    {
        [SerializeField] private FlowEditorTile _flowTilePrefab;

        private Transform _cachedTransform;

        private Dictionary<Vector2Int, FlowEditorTile> _flowTilePositions;
        private GameObjectPool<FlowEditorTile> _flowTilePool;

        private void Awake()
        {
            _cachedTransform = transform;

            _flowTilePositions = new();
            _flowTilePool = new(
                _cachedTransform,
                _flowTilePrefab.gameObject,
                defaultCapacity: 20
            );
        }

        public void CreateFlowTile(Vector2Int tileIndex, Vector2 tilePosition, FlowTileType tileType, Color tileColor)
        {
            FlowEditorTile flowTile = _flowTilePool.Get();
            flowTile.CachedTransform.SetParent(_cachedTransform);
            flowTile.CachedTransform.position = tilePosition;
            flowTile.Init(tileIndex, tileType, tileColor);
            _flowTilePositions.Add(tileIndex, flowTile);
        }

        internal void UpdateFlowTileType(Vector2Int tileIndex, Vector2 tilePosition, FlowTileType tileType)
        {
            if (_flowTilePositions.TryGetValue(tileIndex, out FlowEditorTile flowTile))
            {
                flowTile.UpdateTileType(tileType);
            }
        }

        internal void RemoveFlowTile(Vector2Int tileIndex)
        {
            if (_flowTilePositions.TryGetValue(tileIndex, out FlowEditorTile flowTile))
            {
                _flowTilePositions.Remove(tileIndex);
                _flowTilePool.Release(flowTile);
            }
        }

        internal void UpdateFlowTileColor(Color color)
        {

        }
    }
}
