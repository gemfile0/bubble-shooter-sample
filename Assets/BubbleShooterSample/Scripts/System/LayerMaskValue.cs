using UnityEngine;

namespace BubbleShooterSample
{
    public class LayerMaskValue
    {

        public static readonly int NameLayer_Bubble = LayerMask.NameToLayer("Bubble");
        public static readonly int NameLayer_Wall = LayerMask.NameToLayer("Wall");
        public static readonly int NameLayer_ShooterGround = LayerMask.NameToLayer("ShooterGround");

        public static readonly int HitLayer_WallAndBubble = LayerMask.GetMask("Wall", "Bubble");
        public static readonly int HitLayer_Bubble = LayerMask.GetMask("Bubble");
        public static readonly int HitLayer_Wall = LayerMask.GetMask("Wall");
    }
}
