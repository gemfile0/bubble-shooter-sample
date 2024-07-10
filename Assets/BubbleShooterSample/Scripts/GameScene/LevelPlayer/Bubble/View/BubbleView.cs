using BubbleShooterSample.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;

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

        private GameObjectPool<BubbleTile> _bubbleTilePool;

        private Dictionary<Vector2Int, BubbleTile> _bubbleTileDict;

        private void Awake()
        {
            _bubbleTileDict = new();
            _bubbleTilePool = new(
                CachedTransform,
                _bubbleTilePrefab.gameObject,
                defaultCapacity: 50
            );
        }

        internal BubbleTile CreateBubble(Vector2Int headIndex)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.CachedTransform.SetParent(CachedTransform);
            return bubbleTile;
        }

        internal void MoveBubble(Vector2Int tileIndex, int turn)
        {

        }

    }
}
