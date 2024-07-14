using BubbleShooterSample.GameData;
using BubbleShooterSample.Helper;
using BubbleShooterSample.System;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace BubbleShooterSample
{
    public interface ILootAnimationEndPoint
    {
        Vector3 GetLootAnimtionEndPoint();
        void BeginLootAnimation();
        void EndLootAnimation(long bonusCount);
    }

    public enum LootAnimationType
    {
        AttackPoint
    }

    public class LootAnimationInfo
    {
        public long LootPoint { get; }
        public Vector3 StartPosition { get; }
        public Quaternion StartRotation { get; }
        public Action OnComplete { get; }

        public LootAnimationInfo(long lootPoint, Vector3 startPosition, Quaternion startRotation, Action onComplete)
        {
            LootPoint = lootPoint;
            StartPosition = startPosition;
            StartRotation = startRotation;
            OnComplete = onComplete;
        }
    }

    public interface ILootAnimationTrigger
    {
        event Action<LootAnimationType, LootAnimationInfo> requestLootAnimation;
    }

    public class LootAnimationController : MonoBehaviour, IGameObjectFinderSetter
    {
        [Header("Data")]
        [SerializeField] private LootAnimationData _lootAnimationData;

        [Header("View")]
        [SerializeField] private Transform _itemRendererRoot;
        [SerializeField] private LootAnimationSpriteContainer _attackPointPrefab;
        [SerializeField] private LootAnimationTextContainer _countTextPrefab;

        private IEnumerable<ILootAnimationTrigger> _lootAnimationTriggers;
        private Dictionary<LootAnimationType, ILootAnimationEndPoint> _endPointDict;
        private ObjectPool<LootAnimationItem> _lootAnimationItemPool;

        private GameObjectPool<LootAnimationSpriteContainer> _attackPointRendererPool;
        private GameObjectPool<LootAnimationTextContainer> _countTextPool;

        private Stack<List<LootAnimationSpriteContainer>> _spriteViewListPool;

        private int _itemIndex;

        public void OnGameObjectFinderAwake(IGameObjectFinder finder)
        {
            _lootAnimationTriggers = finder.FindGameObjectOfType<ILootAnimationTrigger>();
        }

        private void Awake()
        {
            _endPointDict = new();
            _lootAnimationItemPool = new(createFunc: () => new LootAnimationItem(),
                                         defaultCapacity: 3);
            _attackPointRendererPool = new(_itemRendererRoot, _attackPointPrefab.gameObject, defaultCapacity: 3);
            _countTextPool = new(_itemRendererRoot, _countTextPrefab.gameObject, defaultCapacity: 3);
            _spriteViewListPool = new();
            _itemIndex = 0;
        }

        private void OnEnable()
        {
            foreach (ILootAnimationTrigger trigger in _lootAnimationTriggers)
            {
                trigger.requestLootAnimation += PlayLootAnimation;
            }
        }

        private void OnDisable()
        {
            foreach (ILootAnimationTrigger trigger in _lootAnimationTriggers)
            {
                trigger.requestLootAnimation += PlayLootAnimation;
            }
        }

        public void AddLootAnimationEndPoint(LootAnimationType type, ILootAnimationEndPoint endPoint)
        {
            _endPointDict.Add(type, endPoint);
        }

        private void PlayLootAnimation(LootAnimationType type, LootAnimationInfo info)
        {
            LootAnimationItem item = _lootAnimationItemPool.Get();
            item.Init(info.LootPoint,
                      info.StartPosition,
                      info.StartRotation,
                      _endPointDict[type],
                      info.OnComplete);

            switch (type)
            {
                case LootAnimationType.AttackPoint:
                    AnimateAttackPoint(item);
                    break;
            }
        }

        private void AnimateAttackPoint(LootAnimationItem item)
        {
            long attackPoint = item.LootPoint;

            LootAnimationSpriteContainer attackPointRenderer = GetSpriteContainer(
                _attackPointRendererPool,
                item.StartPosition,
                item.StartRotation
            );
            attackPointRenderer.Init(_itemIndex);
            IncreaseItemIndex();

            LootAnimationTextContainer countText = GetCountText(new Vector3(item.EndPosition.x, item.EndPosition.y + _lootAnimationData.TextStartOffsetY, item.EndPosition.z));
            IncreaseItemIndex();

            // 연출 시작
            item.LootAnimationEndPoint.BeginLootAnimation();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(attackPointRenderer.SpriteRenderer.DOFade(1f, _lootAnimationData.FadeDuration)
                                                              .SetEase(_lootAnimationData.FadeEase));
            sequence.Insert(0f, attackPointRenderer.CachedTransform.DORotate(Vector3.up, _lootAnimationData.MoveDuration)
                                                                   .SetEase(_lootAnimationData.MoveEase));

            Debug.Log($"AnimateAttackPoint : {item.StartPosition} -> {item.EndPosition}");
            Vector3 diff = item.EndPosition - item.StartPosition;
            Vector3 midPoint = item.StartPosition + diff / 2 + Vector3.up * _lootAnimationData.JumpHeight;
            Vector3[] path = new Vector3[] { item.StartPosition, midPoint, item.EndPosition };
            sequence.Insert(0, attackPointRenderer.CachedTransform.DOPath(path, _lootAnimationData.MoveDuration, _lootAnimationData.PathType)
                                                                  .SetEase(_lootAnimationData.MoveEase));
            sequence.Append(attackPointRenderer.SpriteRenderer.DOFade(0f, _lootAnimationData.FadeDuration)
                                                              .SetEase(_lootAnimationData.FadeEase));

            sequence.JoinCallback(() => countText.TextMeshPro.text = $"-{item.LootPoint}");
            sequence.Join(countText.CachedTransform.DOMoveY(item.EndPosition.y + _lootAnimationData.TextEndOffsetY, _lootAnimationData.TextDuration)
                                                   .SetEase(_lootAnimationData.TextEase));
            // 텍스트 나타나는 타이밍에 연출 완료
            sequence.JoinCallback(() =>
            {
                item.LootAnimationEndPoint.EndLootAnimation(item.LootPoint);
                item.OnComplete?.Invoke();
            });
            sequence.Append(countText.TextMeshPro.DOFade(0f, _lootAnimationData.FadeDuration)
                                                 .SetEase(_lootAnimationData.FadeEase));
            sequence.OnComplete(() =>
            {
                _attackPointRendererPool.Release(attackPointRenderer);

                _countTextPool.Release(countText);
                _lootAnimationItemPool.Release(item);
            });
        }

        private LootAnimationSpriteContainer GetSpriteContainer(
            GameObjectPool<LootAnimationSpriteContainer> itemPool,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 startPositionOffset = default
        )
        {
            LootAnimationSpriteContainer spriteView = itemPool.Get();
            spriteView.CachedTransform.SetParent(_itemRendererRoot);
            spriteView.CachedTransform.position = startPosition + startPositionOffset;
            spriteView.CachedTransform.rotation = startRotation;
            Color originRendererColor = spriteView.SpriteRenderer.color;
            spriteView.SpriteRenderer.color = new Color(originRendererColor.r, originRendererColor.g, originRendererColor.b, 0f);
            return spriteView;
        }

        private LootAnimationTextContainer GetCountText(Vector3 endPosition)
        {
            LootAnimationTextContainer countText = _countTextPool.Get();
            countText.TextMeshPro.text = "";
            countText.CachedTransform.SetParent(_itemRendererRoot);
            countText.CachedTransform.position = endPosition;
            Color originTextColor = countText.TextMeshPro.color;
            countText.TextMeshPro.color = new Color(originTextColor.r, originTextColor.g, originTextColor.b, 1f);
            return countText;
        }

        private void ReleaseCoinRendererList(List<LootAnimationSpriteContainer> spriteviewList)
        {
            _spriteViewListPool.Push(spriteviewList);
        }

        private List<LootAnimationSpriteContainer> GetSpriteViewList()
        {
            if (_spriteViewListPool.Count == 0)
            {
                _spriteViewListPool.Push(new List<LootAnimationSpriteContainer>());
            }
            return _spriteViewListPool.Pop();
        }

        private void IncreaseItemIndex()
        {
            _itemIndex += 1;
            if (_itemIndex == int.MaxValue)
            {
                _itemIndex = 0;
            }
        }
    }
}
