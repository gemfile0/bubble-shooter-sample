using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.GameData
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Bubble Data")]
    public class BubbleData : ScriptableObject
    {
        [SerializeField]
        private List<Color> _bubbleTileColorList = new()
        {
            Color.yellow,
            Color.red,
            Color.blue,
            new Color(0.8f, 0.8f, 0.8f) // lightening color (밝은 회색)
        };

        [Header("Feed")]
        [SerializeField] private Ease _feedBubbleSequenceEase = Ease.InOutSine;
        [SerializeField] private float _moveBubbleDuration = 0.25f;

        [Header("Attack Point")]
        [SerializeField] private float _attackPointSpawnRate = .25f;
        [SerializeField] private int _attackPoint = 5;

        [Header("Fade")]
        [SerializeField] private float _fadeBubbleDuration = 0.25f * .5f;

        [Header("Drop")]
        [SerializeField] private float _droppingGravityScale = 5f;
        [SerializeField] private Vector2 _droppingForceRange = new Vector2(1f, 3f);

        //
        public Ease FeedBubbleSequenceEase => _feedBubbleSequenceEase;
        public float MoveBubbleDuration => _moveBubbleDuration;

        //
        public float AttackPointSpawnRate => _attackPointSpawnRate;
        public int AttackPoint => _attackPoint;

        //
        public float FadeBubbleDuration => _fadeBubbleDuration;

        //
        public float DroppingGravityScale => _droppingGravityScale;
        public Vector2 DroppingForceRange => _droppingForceRange;

        public Color GetRandomBubbleTileColor()
        {
            int randomIndex = Random.Range(0, _bubbleTileColorList.Count);
            return _bubbleTileColorList[randomIndex];
        }
    }
}
