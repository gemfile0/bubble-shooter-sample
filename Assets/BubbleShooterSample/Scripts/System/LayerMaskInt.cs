using UnityEngine;

namespace BubbleShooterSample
{
    public class LayerMaskInt
    {
        public static readonly int BubbleLayer = LayerMask.NameToLayer("Bubble");
        public static readonly int WallLayer = LayerMask.NameToLayer("Wall");
        public static readonly int HitLayer = LayerMask.GetMask("Wall", "Bubble");
    }
}
