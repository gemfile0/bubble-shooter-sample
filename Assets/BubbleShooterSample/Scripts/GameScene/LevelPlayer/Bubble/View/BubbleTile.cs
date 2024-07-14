using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public enum BubbleTileState
    {
        Hanging,
        Shooting,
        Falling
    }

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

        public IEnumerable<SpriteRenderer> SpriteRendererList => _spriteRendererList;

        public Color BubbleColor { get; private set; }

        private Vector3 _velocity;
        private float _gravityScale;
        private Coroutine _shootCoroutine;
        private Coroutine _dropCoroutine;
        private float _radius;
        private List<Transform> _hitBubbleTransformList;
        private SpriteRenderer[] _spriteRendererList;
        private BubbleTileState _state;

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

        private void SetState(BubbleTileState state)
        {
            _state = state;
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
            SetState(BubbleTileState.Shooting);

            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _velocity = direction * speed;
            StopShootCoroutine();
            _shootCoroutine = StartCoroutine(ShootCoroutine());
        }

        public void Drop(float force, float gravityScale)
        {
            SetState(BubbleTileState.Falling);

            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle;
            _velocity = randomDirection * force;
            _gravityScale = gravityScale;

            StopDropCoroutine();
            _dropCoroutine = StartCoroutine(DropCoroutine(gravityScale));
        }

        private void StopShootCoroutine()
        {
            if (_shootCoroutine != null)
            {
                StopCoroutine(_shootCoroutine);
                _shootCoroutine = null;
            }
        }

        private void StopDropCoroutine()
        {
            if (_dropCoroutine != null)
            {
                StopCoroutine(_dropCoroutine);
                _dropCoroutine = null;
            }
        }

        private IEnumerator ShootCoroutine()
        {
            while (true)
            {
                yield return MoveCoroutine();
            }
        }

        private IEnumerator DropCoroutine(float gravityScale)
        {
            while (true)
            {
                _velocity += Vector3.down * gravityScale * Time.fixedDeltaTime;
                yield return MoveCoroutine();
            }
        }

        private IEnumerator MoveCoroutine()
        {
            float avoidColliderOffset = 0.01f;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            int hitLayer = collision.gameObject.layer;
            // A-2. 버블과의 충돌 감지
            if (_state == BubbleTileState.Shooting
                && hitLayer == LayerMaskValue.NameLayerBubble)
            {
                StopShootCoroutine();

                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _velocity = Vector3.zero;

                // 부딪힌 버블들 수집
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

        internal bool IsAlphaValue(float v)
        {
            return _spriteRenderer.color.a == v;
        }
    }
}
