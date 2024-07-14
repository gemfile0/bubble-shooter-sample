using System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class LootAnimationItem
    {
        public long LootPoint { get; private set; }
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }
        public ILootAnimationEndPoint LootAnimationEndPoint { get; private set; }
        public Vector3 EndPosition { get; private set; }
        public Action OnComplete { get; private set; }

        internal void Init(long lootPoint, Vector3 startPosition, Quaternion startRotation, ILootAnimationEndPoint lootAnimationEndPoint, Action onComplete)
        {
            LootPoint = lootPoint;
            StartPosition = startPosition;
            StartRotation = startRotation;
            LootAnimationEndPoint = lootAnimationEndPoint;
            EndPosition = LootAnimationEndPoint.GetLootAnimtionEndPoint();
            OnComplete = onComplete;
        }
    }
}
