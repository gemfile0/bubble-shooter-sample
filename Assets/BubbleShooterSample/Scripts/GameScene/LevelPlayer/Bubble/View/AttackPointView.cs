using System.Collections;
using UnityEngine;

namespace BubbleShooterSample
{
    public class AttackPointView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _circleRadius = .5f;
        [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1f);
        [SerializeField] private Vector2 _delayRange = new Vector2(0f, 0.5f);

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private Transform _spriteTransform;
        private Vector3 _targetPosition;

        public void Init()
        {
            _spriteTransform = transform;
        }

        private void Start()
        {
            StartCoroutine(MoveRandomly());
        }

        private IEnumerator MoveRandomly()
        {
            while (true)
            {
                SetRandomTargetPosition();
                float randomSpeed = Random.Range(_speedRange.x, _speedRange.y);

                while (Vector3.Distance(_spriteTransform.localPosition, _targetPosition) > 0.1f)
                {
                    _spriteTransform.localPosition = Vector3.MoveTowards(
                        current: _spriteTransform.localPosition,
                        target: _targetPosition,
                        maxDistanceDelta: randomSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                float randomDelay = Random.Range(_delayRange.x, _delayRange.y);
                yield return new WaitForSeconds(randomDelay);
            }
        }

        private void SetRandomTargetPosition()
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float x = Mathf.Cos(randomAngle) * _circleRadius;
            float y = Mathf.Sin(randomAngle) * _circleRadius;

            _targetPosition = new Vector3(x, y, _spriteTransform.localPosition.z);
        }

    }
}
