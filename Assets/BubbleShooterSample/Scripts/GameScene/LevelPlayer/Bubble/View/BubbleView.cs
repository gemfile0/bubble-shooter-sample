using BubbleShooterSample.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;
        [SerializeField] private float _moveDuration = 1f;

        public float MoveDuration => _moveDuration;

        private Transform _cachedTransform;

        private GameObjectPool<BubbleTile> _bubbleTilePool;
        private Dictionary<Vector2Int, BubbleTile> _bubbleTileDict;

        private void Awake()
        {
            _cachedTransform = transform;

            _bubbleTilePool = new(
                _cachedTransform,
                _bubbleTilePrefab.gameObject,
                defaultCapacity: 50
            );

            _bubbleTileDict = new();
        }

        public BubbleTile CreateBubbleTile(Vector2Int tileIndex, Color bubbleColor, Vector2 tilePosition)
        {
            BubbleTile bubbleTile = CreateBubbleTile(bubbleColor, tilePosition);
            _bubbleTileDict.Add(tileIndex, bubbleTile);
            return bubbleTile;
        }

        internal BubbleTile CreateBubbleTile(Color bubbleColor, Vector2 tilePosition)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.Init(bubbleColor);
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
            bubbleTile.CachedTransform.position = tilePosition;
            return bubbleTile;
        }

        internal void AddBubbleTile(Vector2Int tileIndex, BubbleTile bubbleTile)
        {
            _bubbleTileDict.Add(tileIndex, bubbleTile);
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
        }

        internal void RemoveBubbleTile(Vector2Int bubbleTileIndex)
        {
            if (_bubbleTileDict.TryGetValue(bubbleTileIndex, out BubbleTile bubbleTile))
            {
                _bubbleTileDict.Remove(bubbleTileIndex);

                _bubbleTilePool.Release(bubbleTile);
            }
        }

        internal BubbleTile GetBubbleTile(Vector2Int bubbleTileIndex)
        {
            if (_bubbleTileDict.TryGetValue(bubbleTileIndex, out BubbleTile bubbleTile) == false)
            {
                Debug.LogWarning($"GetBubbleTile : 타일이 존재하지 않는 인덱스입니다, {bubbleTileIndex}");
            }
            return bubbleTile;
        }
    }
}
