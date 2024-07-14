using DG.Tweening;
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
        [SerializeField] private AttackPointView _attackPointView;

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

        public Color BubbleColor { get; private set; }

        private Vector3 _velocity;
        private Coroutine _moveCoroutine;
        private float _radius;
        private List<Transform> _hitBubbleTransformList;
        private SpriteRenderer[] _spriteRendererList;

        public void Init(Color bubbleColor, bool hasAttackPoint)
        {
            _spriteRendererList = GetComponentsInChildren<SpriteRenderer>();
            BubbleColor = bubbleColor;
            _spriteRenderer.color = BubbleColor;
            SetRendererAlpha(0f);

            _radius = _collider2D.radius * transform.localScale.x;
            _hitBubbleTransformList = new();

            _attackPointView.gameObject.SetActive(hasAttackPoint);
            _attackPointView.Init();
        }

        public void SetRendererAlpha(float value)
        {
            foreach (SpriteRenderer spriteRenderer in _spriteRendererList)
            {
                Color originColor = spriteRenderer.color;
                originColor.a = value;
                spriteRenderer.color = originColor;
            }
        }

        public void SetColliderEnabled(bool value)
        {
            _collider2D.enabled = value;
        }

        public void Shoot(Vector3 direction, float speed)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _velocity = direction * speed;
            StopMoveCoroutine();
            _moveCoroutine = StartCoroutine(MoveCoroutine());
        }

        private void StopMoveCoroutine()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
        }

        private IEnumerator MoveCoroutine()
        {
            float avoidColliderOffset = 0.01f;
            while (true)
            {
                Vector3 nextPosition = CachedTransform.position + _velocity * Time.fixedDeltaTime;

                // A-1. 벽과의 충돌 감지
                Vector3 raycastStartPosition = CachedTransform.position + _velocity.normalized * (_radius + avoidColliderOffset);
                RaycastHit2D wallHit = Physics2D.Raycast(
                    origin: raycastStartPosition,
                    direction: _velocity,
                    distance: _velocity.magnitude * Time.fixedDeltaTime * 2 /* 거리가 짧아서 약간 더 길게 세팅 */,
                    layerMask: LayerMaskValue.HitLayerWall
                );

                if (wallHit.collider != null)
                {
                    _velocity = new Vector3(-1 * _velocity.x, _velocity.y, _velocity.z);
                }

                CachedTransform.position = nextPosition;
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            int hitLayer = collision.gameObject.layer;
            // A-2. 버블과의 충돌 감지
            if (hitLayer == LayerMaskValue.NameLayerBubble)
            {
                StopMoveCoroutine();

                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _velocity = Vector3.zero;

                Vector2 hitPosition = collision.contacts[0].point;

                _hitBubbleTransformList.Clear();
                Collider2D[] colliders = Physics2D.OverlapCircleAll(hitPosition, _radius, LayerMaskValue.HitLayerBubble);
                foreach (var collider in colliders)
                {
                    if (collider != _collider2D)
                    {
                        _hitBubbleTransformList.Add(collider.transform);
                    }
                }

                onHit?.Invoke(_hitBubbleTransformList);
            }
        }

        internal Tween DOFade(float value, float duration)
        {
            Sequence sequence = DOTween.Sequence();
            foreach (SpriteRenderer spriteRenderer in _spriteRendererList)
            {
                sequence.Join(spriteRenderer.DOFade(value, duration));
            }
            return sequence;
        }

        internal bool IsAlphaValue(float v)
        {
            return _spriteRenderer.color.a == v;
        }
    }
}
