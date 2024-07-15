using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Gate Keeper Data")]
    public class GateKeeperData : ScriptableObject
    {
        [SerializeField] private List<Sprite> gateKeeperSkinList;
        [SerializeField] private float healthSliderDuration = .5f;
        [SerializeField] private Ease healthSliderEase = Ease.OutQuad;

        public int LastSkinIndex => gateKeeperSkinList.Count - 1;
        public float HealthSliderDuration => healthSliderDuration;
        public Ease HealthSliderEase => healthSliderEase;

        public Sprite GetGateKeeperSprite(int skinIndex)
        {
            return gateKeeperSkinList[skinIndex];
        }
    }
}
