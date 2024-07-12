using UnityEngine;

namespace BubbleShooterSample
{
    public class LayerMaskValue
    {

        public static readonly int NameLayerBubble = LayerMask.NameToLayer("Bubble");
        public static readonly int NameLayerWall = LayerMask.NameToLayer("Wall");

        public static readonly int HitLayerAll = LayerMask.GetMask("Wall", "Bubble");
        public static readonly int HitLayerBubble = LayerMask.GetMask("Bubble");
        public static readonly int HitLayerWall = LayerMask.GetMask("Wall");
    }
}
