using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _collider2D;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        public event Action<IEnumerable<Transform>> onHit;

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

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private Vector3 _velocity;
        private bool _isMoving = false;
        private Coroutine _moveCoroutine;
        private float _radius;
        private List<Transform> _hitBubbleTransformList;

        public void Init(Color bubbleColor)
        {
            bubbleColor.a = 0f;
            _spriteRenderer.color = bubbleColor;

            _rigidbody2D.isKinematic = true;
            _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _radius = _collider2D.radius * transform.localScale.x;
            //Debug.Log($"Init : {_radius}");
            _hitBubbleTransformList = new();
        }

        public void SetRendererAlpha(float value)
        {
            Color bubbleColor = _spriteRenderer.color;
            bubbleColor.a = value;
            _spriteRenderer.color = bubbleColor;
        }

        public void SetColliderEnabled(bool value)
        {
            _collider2D.enabled = value;
        }

        public void Shoot(Vector3 direction, float speed)
        {
            _velocity = direction * speed;
            _isMoving = true;
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            _moveCoroutine = StartCoroutine(MoveCoroutine());
        }

        private IEnumerator MoveCoroutine()
        {
            while (_isMoving)
            {
                Vector3 nextPosition = CachedTransform.position + _velocity * Time.deltaTime;
                Vector3 raycastStartPosition = CachedTransform.position + _velocity.normalized * (_radius + 0.01f);
                RaycastHit2D hit = Physics2D.Raycast(
                    origin: raycastStartPosition,
                    direction: _velocity,
                    distance: _velocity.magnitude * Time.deltaTime,
                    layerMask: LayerMaskValue.AllHitLayer
                );

                if (hit.collider != null)
                {
                    Vector2 hitPosition = hit.point;
                    int hitLayer = hit.collider.gameObject.layer;
                    if (hitLayer == LayerMaskValue.BubbleNameLayer)
                    {
                        _velocity = Vector3.zero;
                        _isMoving = false;
                        nextPosition = hitPosition;

                        _hitBubbleTransformList.Clear();
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitPosition, _radius, LayerMaskValue.BubbleHitLayer);
                        foreach (var collider in colliders)
                        {
                            if (collider != _collider2D)
                            {
                                _hitBubbleTransformList.Add(collider.transform);
                            }
                        }
                    }
                    else if (hitLayer == LayerMaskValue.WallNameLayer)
                    {
                        _velocity = new Vector3(-1 * _velocity.x, _velocity.y, _velocity.z);
                        nextPosition = hitPosition + (Vector2)(_velocity.normalized * 0.01f); // 작은 오프셋을 추가하여 충돌을 피함
                    }
                }

                CachedTransform.position = nextPosition;
                yield return null;
            }

            onHit?.Invoke(_hitBubbleTransformList);
        }
    }
}
