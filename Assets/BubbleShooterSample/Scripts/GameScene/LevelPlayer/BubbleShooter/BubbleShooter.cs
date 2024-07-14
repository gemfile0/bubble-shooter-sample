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

    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private BubblePresenter _bubblePresenter;

        [SerializeField] private BubbleShooterData _bubbleShooterData;

        [Header("View")]
        [SerializeField] private Transform _shooterTransform;
        [SerializeField] private float _shooterPositionY = -5f;

        [Header("Guide Line")]
        [SerializeField] private LineRenderer _guideLineRenderer;

        [Header("Guide Circle")]
        [SerializeField] private Transform _guideCircleTransform;

        public event Func<Vector2, ClosestTileInfo> requestGettingClosestTileInfo;

        private const int MaxReflections = 2;
        private const float ColliderOffset = 0.01f;

        private BubbleTile _bubbleTile;
        private Vector3 _shooterTopPosition;
        private List<Vector3> _linePoints;
        private Vector3 _shootDirection;
        private IEnumerable<Transform> _hitBubbleTransforms;

        private Vector3 _totalGridSize;
        private float _horizontalSpacing;
        private BubbleShooterState _currentState;
        private GameObject _guideCircleObject;

        private void Awake()
        {
            _linePoints = new();
            SetState(BubbleShooterState.Wait);
            _guideCircleObject = _guideCircleTransform.gameObject;
        }

        public void Init(Vector3 totalGridSize, float horizontalSpacing)
        {
            _totalGridSize = totalGridSize;
            _horizontalSpacing = horizontalSpacing;

            SetupShooter(null);
        }

        internal void SetReadyToShoot()
        {
            if (_bubbleTile == null)
            {
                SetupShooter(SetStateAsReadyToShoot);
            }
            else
            {
                SetStateAsReadyToShoot();
            }
        }

        private void SetupShooter(TweenCallback OnSetupShooterComplete)
        {
            Vector2 gridSizeHalf = _totalGridSize / 2f;
            float horizontalSpacingHalf = _horizontalSpacing / 2f;

            _shooterTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _shooterPositionY);

            // A-1. 5시 방향에서 버블 생성
            float radius = 1.9f / 2;
            Vector3 startPosition = _shooterTransform.position + new Vector3(Mathf.Cos(300 * Mathf.Deg2Rad), Mathf.Sin(300 * Mathf.Deg2Rad)) * radius;
            _shooterTopPosition = _shooterTransform.position + new Vector3(Mathf.Cos(90 * Mathf.Deg2Rad), Mathf.Sin(90 * Mathf.Deg2Rad)) * radius;

            _bubbleTile = _bubblePresenter.CreateBubbleTile(startPosition);
            _bubbleTile.CachedTransform.SetParent(_shooterTransform);

            // A-2. 경로 설정: 시계 반대 방향으로 원형 경로를 그리며 이동
            Vector3[] path = new Vector3[]
            {
                startPosition,
                _shooterTransform.position + new Vector3(Mathf.Cos(330 * Mathf.Deg2Rad), Mathf.Sin(330 * Mathf.Deg2Rad)) * radius,
                _shooterTransform.position + new Vector3(Mathf.Cos(0 * Mathf.Deg2Rad), Mathf.Sin(0 * Mathf.Deg2Rad)) * radius,
                _shooterTransform.position + new Vector3(Mathf.Cos(30 * Mathf.Deg2Rad), Mathf.Sin(30 * Mathf.Deg2Rad)) * radius,
                _shooterTransform.position + new Vector3(Mathf.Cos(60 * Mathf.Deg2Rad), Mathf.Sin(60 * Mathf.Deg2Rad)) * radius,
                _shooterTopPosition
            };

            float snappingDuration = _bubbleShooterData.BubbleSnappingDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_bubbleTile.CachedTransform.DOPath(path, snappingDuration, PathType.CatmullRom));
            foreach (SpriteRenderer spriteRenderer in _bubbleTile.SpriteRendererList)
            {
                sequence.Join(spriteRenderer.DOFade(1f, snappingDuration));
            }
            sequence.OnComplete(OnSetupShooterComplete);
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

                // 부딪힌 버블들이 움찔하는 애니메이션 
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
            sequence.InsertCallback(bubbleBumpDuration, () => _bubblePresenter.AddBubbleTile(closestTileInfo.Index, bubbleTile));
        }

        private void Update()
        {
            // B-1. ReadyToShoot 상태가 아니면 리턴
            if (_currentState != BubbleShooterState.ReadyToShoot)
            {
                return;
            }

            // B-2. 지정한 범위의 각도에 들어오지 않으면 리턴
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            _shootDirection = (mousePosition - _shooterTopPosition).normalized;
            float guidelineAngle = Vector2.Angle(Vector2.right, _shootDirection);
            Vector2 guidelineAngleRange = _bubbleShooterData.GuidelineAngleRange;
            if (guidelineAngle < guidelineAngleRange.x || guidelineAngle > guidelineAngleRange.y)
            {
                _guideLineRenderer.positionCount = 0;
                _guideCircleObject.SetActive(false);
                return;
            }

            // B-3. 가이드라인 그리기
            if (Input.GetMouseButton(0))
            {
                _guideCircleObject.SetActive(false);

                DrawGuideLine(_shooterTopPosition, _shootDirection);
            }
            // B-4. 버블 쏘기
            else if (Input.GetMouseButtonUp(0))
            {
                _guideCircleObject.SetActive(false);

                _guideLineRenderer.positionCount = 0;
                ShootBubble(_shootDirection);
                SetState(BubbleShooterState.Shooting);
            }
            else
            {
                _guideCircleObject.SetActive(true);
                _guideCircleTransform.position = mousePosition;
                _guideLineRenderer.positionCount = 0;
            }
        }

        private void DrawGuideLine(Vector3 startPosition, Vector3 direction)
        {
            _linePoints.Clear();
            _linePoints.Add(startPosition);

            Vector3 currentPosition = startPosition;
            Vector3 currentDirection = direction;
            int reflections = 0;
            float remainingDistance = _bubbleShooterData.GuidelineMaxDistance;
            while (reflections < MaxReflections)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    origin: currentPosition,
                    direction: currentDirection,
                    distance: remainingDistance,
                    layerMask: LayerMaskValue.HitLayer_WallAndBubble
                );

                if (hit.collider != null)
                {
                    Vector2 hitPosition = hit.point;
                    _linePoints.Add(hitPosition);
                    float distanceToHit = Vector2.Distance(currentPosition, hitPosition);
                    remainingDistance -= distanceToHit;
                    currentPosition = hitPosition;

                    // C-1. Bubble 콜라이더에 부딪힌 경우, 렌더링 멈춤
                    int hitLayer = hit.collider.gameObject.layer;
                    if (hitLayer == LayerMaskValue.NameLayer_Bubble)
                    {
                        break;
                    }

                    // C-2. Wall 콜라이더에 부딪힌 경우, 반사 효과 적용
                    if (hitLayer == LayerMaskValue.NameLayer_Wall)
                    {
                        currentDirection = new Vector2(-1 * currentDirection.x, currentDirection.y);
                        currentPosition += currentDirection * ColliderOffset;
                        reflections++;
                    }
                }
                else
                {
                    _linePoints.Add(currentPosition + currentDirection * remainingDistance);
                    break;
                }
            }

            _guideLineRenderer.positionCount = _linePoints.Count;
            _guideLineRenderer.SetPositions(_linePoints.ToArray());
        }

        private void ShootBubble(Vector3 direction)
        {
            float bubbleSpeed = _bubbleShooterData.BubbleSpeed;

            _bubbleTile.CachedTransform.SetParent(null);
            _bubbleTile.SetColliderEnabled(true);
            _bubbleTile.Shoot(direction, bubbleSpeed);
        }
    }
}
