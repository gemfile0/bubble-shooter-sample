using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class FlowView : MonoBehaviour
    {
        [SerializeField] private FlowTile _flowTilePrefab;

        private Transform CachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }
                return _cachedTransform;
            }
        }
        private Transform _cachedTransform;

        private List<Vector2> _flowTilePositions = new List<Vector2>(); // 플로우 타일 위치 리스트

        public void CreateFlowTile(Vector2 position, FlowTileType tileType)
        {
            FlowTile flowTile = Instantiate(_flowTilePrefab, position, Quaternion.identity, CachedTransform);
            flowTile.Init(tileType);
            _flowTilePositions.Add(position);
        }
    }
}
