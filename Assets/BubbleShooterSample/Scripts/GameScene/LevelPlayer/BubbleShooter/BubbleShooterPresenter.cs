using BubbleShooterSample.System;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public enum BubbleShooterState
    {
        Wait,
        ReadyToShoot,
        Shooting,
        Shooted,
    }

    public class BubbleShooterPresenter : MonoBehaviour,
        ILevelRestorable
    {
        [SerializeField] private BubbleShooterData _bubbleShooterData;
        [SerializeField] private BubbleShooterModel _bubbleShooterModel;
        [SerializeField] private BubbleShooterView _bubbleShooterView;

        public event Func<Vector2, ClosestTileInfo> requestGettingClosestTileInfo;
        public event Func<Vector2, BubbleTile> requestCreatingBubbleTile;
        public event Action<Vector2Int, BubbleTile> requestAddingBubbleTile;

        private Vector3 _totalGridSize;
        private float _horizontalSpacing;

        private BubbleShooterState _currentState;

        private Vector3 _shooterTopPosition;
        private BubbleTile _bubbleTile;
        private IEnumerable<Transform> _hitBubbleTransforms;

        public LevelDataId RestoreLevelID => LevelDataId.BubbleShooter;

        public void Init(Vector3 totalGridSize, float horizontalSpacing)
        {
            _totalGridSize = totalGridSize;
            _horizontalSpacing = horizontalSpacing;

            UpdateShooterPosition();
            ChargeBubble(null);
            SetState(BubbleShooterState.Wait);

            // _shooterTopPosition 값이 위의 ChargeBubble(null) 메서드에서 초기화 됩니다.
            _bubbleShooterView.Init(
                _shooterTopPosition,
                _bubbleShooterData.GuidelineAngleRange,
                _bubbleShooterData.GuidelineMaxDistance
            );
        }

        private void OnEnable()
        {
            _bubbleShooterModel.onBubbleCountRestored += _bubbleShooterView.UpdateBubbleCount;
            _bubbleShooterModel.onBubbleCountConsumed += _bubbleShooterView.UpdateBubbleCount;

            _bubbleShooterView.onShootBubble += ShootBubble;
        }

        private void OnDisable()
        {
            _bubbleShooterModel.onBubbleCountRestored -= _bubbleShooterView.UpdateBubbleCount;
            _bubbleShooterModel.onBubbleCountConsumed -= _bubbleShooterView.UpdateBubbleCount;

            _bubbleShooterView.onShootBubble -= ShootBubble;
        }

        public void RestoreLevelData(string dataStr)
        {
            _bubbleShooterModel.RestoreLevelData(dataStr);
        }

        private void UpdateShooterPosition()
        {
            Vector2 gridSizeHalf = _totalGridSize / 2f;
            float horizontalSpacingHalf = _horizontalSpacing / 2f;

            _bubbleShooterView.CachedTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _bubbleShooterData.BubbleShooterPositionY);
        }

        internal void SetReadyToShoot()
        {
            if (_bubbleTile == null)
            {
                ChargeBubble(SetStateAsReadyToShoot);
            }
            else
            {
                SetStateAsReadyToShoot();
            }
        }

        private void ChargeBubble(TweenCallback OnChargeBubbleComplete)
        {
            bool consumed = _bubbleShooterModel.ConsumeBubbleCount();
            if (consumed == false)
            {
                return;
            }

            Transform shooterTransform = _bubbleShooterView.CachedTransform;
            Vector3 shooterPosition = shooterTransform.position;

            // A-1. 5시 방향에서 버블 생성
            float radius = 1.9f / 2;
            Vector3 startPosition = shooterPosition + new Vector3(Mathf.Cos(300 * Mathf.Deg2Rad), Mathf.Sin(300 * Mathf.Deg2Rad)) * radius;
            _shooterTopPosition = shooterPosition + new Vector3(Mathf.Cos(90 * Mathf.Deg2Rad), Mathf.Sin(90 * Mathf.Deg2Rad)) * radius;

            _bubbleTile = requestCreatingBubbleTile(startPosition);
            _bubbleTile.CachedTransform.SetParent(shooterTransform);

            // A-2. 경로 설정: 시계 반대 방향으로 원형 경로를 그리며 이동
            Vector3[] path = new Vector3[]
            {
                startPosition,
                shooterPosition + new Vector3(Mathf.Cos(330 * Mathf.Deg2Rad), Mathf.Sin(330 * Mathf.Deg2Rad)) * radius,
                shooterPosition + new Vector3(Mathf.Cos(0 * Mathf.Deg2Rad), Mathf.Sin(0 * Mathf.Deg2Rad)) * radius,
                shooterPosition + new Vector3(Mathf.Cos(30 * Mathf.Deg2Rad), Mathf.Sin(30 * Mathf.Deg2Rad)) * radius,
                shooterPosition + new Vector3(Mathf.Cos(60 * Mathf.Deg2Rad), Mathf.Sin(60 * Mathf.Deg2Rad)) * radius,
                _shooterTopPosition
            };

            float snappingDuration = _bubbleShooterData.BubbleSnappingDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_bubbleTile.CachedTransform.DOPath(path, snappingDuration, PathType.CatmullRom));
            foreach (SpriteRenderer spriteRenderer in _bubbleTile.SpriteRendererList)
            {
                sequence.Join(spriteRenderer.DOFade(1f, snappingDuration));
            }
            sequence.OnComplete(OnChargeBubbleComplete);
        }

        private void SetStateAsReadyToShoot()
        {
            SetState(BubbleShooterState.ReadyToShoot);
            _bubbleTile.SetColliderEnabled(false);
            _bubbleTile.onHit += OnBubbleHit;
        }

        private void SetState(BubbleShooterState state)
        {
            Debug.Log($"SetState : {_currentState} -> {state}");
            _currentState = state;

            _bubbleShooterView.InputEnabled = state == BubbleShooterState.ReadyToShoot;
        }

        private void OnBubbleHit(IEnumerable<Transform> hitBubbleTransforms)
        {
            SetState(BubbleShooterState.Shooted);

            _bubbleTile.onHit -= OnBubbleHit;
            _hitBubbleTransforms = hitBubbleTransforms;

            BubbleTile bubbleTile = ConsumeBubbleTile();
            SnapToGrid(bubbleTile);
            ReactToHit(bubbleTile);
        }

        private BubbleTile ConsumeBubbleTile()
        {
            BubbleTile result = _bubbleTile;
            _bubbleTile = null;
            return result;
        }

        private void ReactToHit(BubbleTile bubbleTile)
        {
            Vector3 hitPosition = bubbleTile.CachedTransform.position;

            float bubbleBumpDistance = _bubbleShooterData.BubbleBumpDistance;
            float bubbleBumpDuration = _bubbleShooterData.BubbleBumpDuration;
            foreach (Transform bubbleTransform in _hitBubbleTransforms)
            {
                Vector3 bubblePosition = bubbleTransform.position;
                Vector3 direction = (bubblePosition - hitPosition).normalized;
                Vector3 targetPosition = bubblePosition + direction * bubbleBumpDistance;

                // 부딪힌 버블들이 부딪힌 방향으로 살짝 움찔하는 연출 
                Sequence sequence = DOTween.Sequence();
                sequence.Append(bubbleTransform.DOMove(targetPosition, bubbleBumpDuration))
                        .Append(bubbleTransform.DOMove(bubblePosition, bubbleBumpDuration));
            }
        }

        private void SnapToGrid(BubbleTile bubbleTile)
        {
            ClosestTileInfo closestTileInfo = requestGettingClosestTileInfo.Invoke(bubbleTile.CachedTransform.position);

            float bubbleSnappingDuration = _bubbleShooterData.BubbleSnappingDuration;
            float bubbleBumpDuration = _bubbleShooterData.BubbleBumpDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(bubbleTile.CachedTransform.DOMove(closestTileInfo.Position, bubbleSnappingDuration).SetEase(_bubbleShooterData.BubbleSnappingEase));
            sequence.InsertCallback(bubbleBumpDuration, () => requestAddingBubbleTile(closestTileInfo.Index, bubbleTile));
        }

        private void ShootBubble(Vector3 direction)
        {
            SetState(BubbleShooterState.Shooting);

            float bubbleSpeed = _bubbleShooterData.BubbleSpeed;

            _bubbleTile.CachedTransform.SetParent(null);
            _bubbleTile.SetColliderEnabled(true);
            _bubbleTile.Shoot(direction, bubbleSpeed);
        }
    }
}
