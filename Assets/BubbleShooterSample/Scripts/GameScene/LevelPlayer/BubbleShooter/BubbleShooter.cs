using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private BubblePresenter _bubblePresenter;
        [SerializeField] private GridView _gridView;

        [Header("Shooter Transform")]
        [SerializeField] private Transform _shooterTransform;
        [SerializeField] private float _shooterPositionY = -5f;
        [SerializeField] private float _bubbleSpeed = 10f;

        [Header("Line Renderer")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxDistance = 5f;

        private const int MaxReflections = 2;
        private const float ColliderOffset = 0.01f;

        private BubbleTile _bubbleTile;
        private Vector3 _shooterTopPosition;
        private List<Vector3> _linePoints;
        private Vector3 _shootDirection;

        private void Awake()
        {
            _linePoints = new();
        }

        public void Init()
        {
            SetupShooter();
        }

        private void SetupShooter()
        {
            Vector2 gridSizeHalf = _gridView.GetTotalGridSize() / 2f;
            float horizontalSpacingHalf = _gridView.HorizontalSpacing / 2f;

            _shooterTransform.position = new Vector2(gridSizeHalf.x - horizontalSpacingHalf, _shooterPositionY);
            _shooterTopPosition = _shooterTransform.position + new Vector3(0, 1.9f / 2, 0);
            _bubbleTile = _bubblePresenter.CreateBubbleTile(_shooterTransform.position);
            _bubbleTile.CachedTransform.SetParent(_shooterTransform);
            _bubbleTile.CachedTransform.position = _shooterTopPosition;
            _bubbleTile.SetRendererAlpha(1f);
            _bubbleTile.SetColliderEnabled(false);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;

                _shootDirection = (mousePosition - _shooterTopPosition).normalized;
                DrawGuideLine(_shooterTopPosition, _shootDirection);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _lineRenderer.positionCount = 0;
                ShootBubble(_shootDirection);
            }
            else
            {
                _lineRenderer.positionCount = 0;
            }
        }

        private void DrawGuideLine(Vector3 startPosition, Vector3 direction)
        {
            _linePoints.Clear();
            _linePoints.Add(startPosition);

            Vector3 currentPosition = startPosition;
            Vector3 currentDirection = direction;
            int reflections = 0;
            float remainingDistance = _maxDistance;
            while (reflections < MaxReflections)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    origin: currentPosition,
                    direction: currentDirection,
                    distance: remainingDistance,
                    layerMask: LayerMaskInt.HitLayer
                );

                if (hit.collider != null)
                {
                    Vector2 hitPosition = hit.point;
                    _linePoints.Add(hitPosition);
                    float distanceToHit = Vector3.Distance(currentPosition, hitPosition);
                    remainingDistance -= distanceToHit;
                    currentPosition = hitPosition;

                    // Bubble 콜라이더에 부딪힌 경우, 렌더링 멈춤
                    int hitLayer = hit.collider.gameObject.layer;
                    if (hitLayer == LayerMaskInt.BubbleLayer)
                    {
                        break;
                    }

                    // Wall 콜라이더에 부딪힌 경우, 반사 효과 적용
                    if (hitLayer == LayerMaskInt.WallLayer)
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

            _lineRenderer.positionCount = _linePoints.Count;
            _lineRenderer.SetPositions(_linePoints.ToArray());
        }

        private void ShootBubble(Vector3 direction)
        {
            _bubbleTile.CachedTransform.SetParent(null);
            _bubbleTile.SetColliderEnabled(true);
            _bubbleTile.Shoot(direction, _bubbleSpeed);
        }
    }
}
