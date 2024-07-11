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

        [Header("Line Renderer")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxDistance = 5f;
        [SerializeField] private int _maxReflections = 5;
        [SerializeField] private LayerMask _hitLayer;
        [SerializeField] private float _colliderOffset = 0.01f;

        private BubbleTile _bubbleTile;
        private Vector3 _shooterTopPosition;
        private List<Vector3> _linePoints;

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

                Vector3 direction = (mousePosition - _shooterTopPosition).normalized;
                DrawGuideLine(_shooterTopPosition, direction);
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
            while (reflections < _maxReflections)
            {
                RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentDirection, remainingDistance, _hitLayer);

                if (hit.collider != null)
                {
                    Vector2 hitPosition = hit.point;
                    _linePoints.Add(hitPosition);
                    float distanceToHit = Vector3.Distance(currentPosition, hitPosition);
                    remainingDistance -= distanceToHit;
                    currentPosition = hitPosition;

                    // Bubble 콜라이더에 부딪힌 경우, 렌더링 멈춤
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Bubble"))
                    {
                        break;
                    }

                    // Wall 콜라이더에 부딪힌 경우, 반사 효과 적용
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        currentDirection = new Vector2(-currentDirection.x, currentDirection.y);
                        currentPosition += currentDirection * _colliderOffset;
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
    }
}
