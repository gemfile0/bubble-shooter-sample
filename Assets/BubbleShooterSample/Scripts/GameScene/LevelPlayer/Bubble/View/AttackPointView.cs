using System.Collections;
using UnityEngine;

namespace BubbleShooterSample
{
    public class AttackPointView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _circleRadius = .5f;
        [SerializeField] private float _speed = 1f;

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

                while (Vector3.Distance(_spriteTransform.localPosition, _targetPosition) > 0.1f)
                {
                    _spriteTransform.localPosition = Vector3.MoveTowards(
                        current: _spriteTransform.localPosition,
                        target: _targetPosition,
                        maxDistanceDelta: _speed * Time.deltaTime
                    );
                    yield return null;
                }

                yield return new WaitForSeconds(1f);
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
