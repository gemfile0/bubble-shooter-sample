using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Gate Keeper Data")]
    public class GateKeeperData : ScriptableObject
    {
        [SerializeField] private List<Sprite> gateKeeperSkinList;

        public int LastSkinIndex => gateKeeperSkinList.Count - 1;

        public Sprite GetGateKeeperSkin(int skinIndex)
        {
            return gateKeeperSkinList[skinIndex];
        }
    }
}
