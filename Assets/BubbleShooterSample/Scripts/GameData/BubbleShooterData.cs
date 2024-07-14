using DG.Tweening;
using UnityEngine;

namespace BubbleShooterSample
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Bubble Shooter Data")]
    public class BubbleShooterData : ScriptableObject
    {
        [Header("Guideline")]
        [SerializeField] private Vector2 _guidelineAngleRange = new Vector2(30, 150);
        [SerializeField] private float _guidelineMaxDistance = 5f;

        [Header("Bubble")]
        [SerializeField] private float _bubbleSpeed = 10f;
        [SerializeField] private float _bubbleSnappingDuration = .25f;
        [SerializeField] private float _bubbleBumpDuration = .25f * .5f;
        [SerializeField] private Ease _bubbleSnappingEase = Ease.OutQuad;
        [SerializeField] private float _bubbleBumpDistance = 0.1f;

        public Vector2 GuidelineAngleRange => _guidelineAngleRange;
        public float GuidelineMaxDistance => _guidelineMaxDistance;

        public float BubbleSpeed => _bubbleSpeed;
        public float BubbleSnappingDuration => _bubbleSnappingDuration;
        public Ease BubbleSnappingEase => _bubbleSnappingEase;
        public float BubbleBumpDistance => _bubbleBumpDistance;
        public float BubbleBumpDuration => _bubbleBumpDuration;
    }
}
