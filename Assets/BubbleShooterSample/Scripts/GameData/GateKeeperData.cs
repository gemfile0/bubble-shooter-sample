using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Gate Keeper Data")]
    public class GateKeeperData : ScriptableObject
    {
        [SerializeField] private List<Sprite> _gateKeeperSkinList;
        [SerializeField] private float _healthSliderDuration = .5f;
        [SerializeField] private Ease _healthSliderEase = Ease.OutQuad;

        public int LastSkinIndex => _gateKeeperSkinList.Count - 1;
        public float HealthSliderDuration => _healthSliderDuration;
        public Ease HealthSliderEase => _healthSliderEase;

        public Sprite GetGateKeeperSprite(int skinIndex)
        {
            Sprite result = null;
            if (skinIndex < _gateKeeperSkinList.Count)
            {
                result = _gateKeeperSkinList[skinIndex];
            }
            else
            {
                Debug.LogWarning($"보유한 스킨 개수를 초과했습니다 : {skinIndex}");
            }
            return result;
        }
    }
}
