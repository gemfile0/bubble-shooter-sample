using BubbleShooterSample.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleView : MonoBehaviour
    {
        [SerializeField] private BubbleTile _bubbleTilePrefab;
        [SerializeField] private float _moveDuration = 1f;
        [SerializeField]
        private List<Color> _bubbleTileColorList = new()
        {
            Color.yellow,
            Color.red,
            Color.blue,
            new Color(0.8f, 0.8f, 0.8f) // lightening color (밝은 회색)
        };

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

        private void Awake()
        {
            _bubbleTilePool = new(
                CachedTransform,
                _bubbleTilePrefab.gameObject,
                defaultCapacity: 50
            );
        }

        internal BubbleTile CreateBubble(Vector2 tilePosition)
        {
            BubbleTile bubbleTile = _bubbleTilePool.Get();
            bubbleTile.Init(GetRandomBubbleTileColor(), _moveDuration);
            bubbleTile.CachedTransform.SetParent(CachedTransform);
            bubbleTile.CachedTransform.position = tilePosition;
            return bubbleTile;
        }

        public Color GetRandomBubbleTileColor()
        {
            int randomIndex = Random.Range(0, _bubbleTileColorList.Count);
            return _bubbleTileColorList[randomIndex];
        }
    }
}
