using UnityEngine;

namespace BubbleShooterSample
{
    public class LayerMaskValue
    {
        public static readonly int BubbleNameLayer = LayerMask.NameToLayer("Bubble");
        public static readonly int WallNameLayer = LayerMask.NameToLayer("Wall");
        public static readonly int AllHitLayer = LayerMask.GetMask("Wall", "Bubble");
        public static readonly int BubbleHitLayer = LayerMask.GetMask("Bubble");
    }
}
