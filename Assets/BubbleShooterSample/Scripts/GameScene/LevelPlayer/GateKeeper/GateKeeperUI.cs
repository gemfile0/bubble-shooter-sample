using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterSample
{
    public class GateKeeperUI : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;

        internal void InitHealthSlider(int healthPoint)
        {
            _healthSlider.maxValue = healthPoint;
            _healthSlider.value = healthPoint;
        }

        internal void UpdateHealthSlider(int healthPoint, float sliderDuration, Ease sliderEase)
        {
            _healthSlider.DOValue(healthPoint, sliderDuration)
                         .SetEase(sliderEase);
        }
    }
}
