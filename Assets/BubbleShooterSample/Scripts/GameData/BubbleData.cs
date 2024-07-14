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
        [SerializeField] private Ease _feedBubbleSequenceEase = Ease.InOutSine;
        [SerializeField] private float _moveBubbleDuration = 0.25f;
        [SerializeField] private float _fadeBubbleDuration = 0.25f * .5f;

        public Ease FeedBubbleSequenceEase => _feedBubbleSequenceEase;
        public float MoveBubbleDuration => _moveBubbleDuration;
        public float FadeBubbleDuration => _fadeBubbleDuration;

        public Color GetRandomBubbleTileColor()
        {
            int randomIndex = Random.Range(0, _bubbleTileColorList.Count);
            return _bubbleTileColorList[randomIndex];
        }
    }
}
