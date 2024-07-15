using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleShooterView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _guideLineRenderer;
        [SerializeField] private Transform _guideCircleTransform;
        [SerializeField] private TextMeshPro _bubbleCountText;

        public event Action<Vector3> onShootBubble;

        public Transform CachedTransform
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

        public bool InputEnabled { private get; set; }

        private const int MaxReflections = 2;
        private const float ColliderOffset = 0.01f;

        private GameObject _guideCircleObject;
        private Vector3 _shooterTopPosition;
        private Vector2 _guidelineAngleRange;
        private float _guidelineMaxDistance;

        private List<Vector3> _linePoints;

        public void Init(Vector3 shooterTopPosition, Vector2 guidelineAngleRange, float guidelineMaxDistance)
        {
            _shooterTopPosition = shooterTopPosition;
            _guidelineAngleRange = guidelineAngleRange;
            _guidelineMaxDistance = guidelineMaxDistance;

            _linePoints = new();

            _guideCircleObject = _guideCircleTransform.gameObject;
        }

        internal void UpdateBubbleCount(int value)
        {
            _bubbleCountText.text = value.ToString();
        }

        private void Update()
        {
            if (InputEnabled == false)
            {
                return;
            }

            // B-2. 지정한 범위의 각도에 들어오지 않으면 리턴
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            Vector3 shootDirection = (mousePosition - _shooterTopPosition).normalized;
            float guidelineAngle = Vector2.Angle(Vector2.right, shootDirection);
            if (guidelineAngle < _guidelineAngleRange.x || guidelineAngle > _guidelineAngleRange.y)
            {
                _guideLineRenderer.positionCount = 0;
                _guideCircleObject.SetActive(false);
                return;
            }

            // B-3. 가이드라인 그리기
            if (Input.GetMouseButton(0))
            {
                _guideCircleObject.SetActive(false);

                DrawGuideLine(_shooterTopPosition, shootDirection);
            }
            // B-4. 버블 쏘기
            else if (Input.GetMouseButtonUp(0))
            {
                _guideCircleObject.SetActive(false);
                _guideLineRenderer.positionCount = 0;

                onShootBubble?.Invoke(shootDirection);
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
            float remainingDistance = _guidelineMaxDistance;
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
    }
}
