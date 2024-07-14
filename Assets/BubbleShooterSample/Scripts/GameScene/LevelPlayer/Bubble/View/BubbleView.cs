using BubbleShooterSample.Helper;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;

        private Transform _cachedTransform;

        private GameObjectPool<BubbleTile> _bubbleTilePool;
        private Dictionary<int, BubbleTile> _bubbleTileDict;

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

        public BubbleTile GetOrCreateBubbleTile(int tileIndex, Color bubbleColor, bool hasAttackPoint, Vector2 tilePosition)
        {
            if (_bubbleTileDict.TryGetValue(tileIndex, out BubbleTile bubbleTile) == false)
            {
                //Debug.Log("CreateBubbleTile");
                bubbleTile = CreateBubbleTile(bubbleColor, hasAttackPoint, tilePosition);
                _bubbleTileDict.Add(tileIndex, bubbleTile);
            }

            return bubbleTile;
        }

        internal BubbleTile CreateBubbleTile(Color bubbleColor, bool hasAttackPoint, Vector2 tilePosition)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.Init(bubbleColor, hasAttackPoint);
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
            bubbleTile.CachedTransform.position = tilePosition;
            return bubbleTile;
        }

        internal void AddBubbleTile(int tileID, BubbleTile bubbleTile)
        {
            _bubbleTileDict.Add(tileID, bubbleTile);
            bubbleTile.CachedTransform.SetParent(_cachedTransform);
        }

        internal void RemoveBubbleTile(int tileID)
        {
            if (_bubbleTileDict.TryGetValue(tileID, out BubbleTile bubbleTile))
            {
                _bubbleTileDict.Remove(tileID);

                bubbleTile.DOKill();
                bubbleTile.CachedTransform.position = Vector3.zero;
                _bubbleTilePool.Release(bubbleTile);
            }
        }

        internal BubbleTile GetBubbleTile(int tileID)
        {
            if (_bubbleTileDict.TryGetValue(tileID, out BubbleTile bubbleTile) == false)
            {
                Debug.LogWarning($"GetBubbleTile : 타일이 존재하지 않는 인덱스입니다, {tileID}");
            }
            return bubbleTile;
        }
    }
}
