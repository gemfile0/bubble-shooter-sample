using System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperModel : MonoBehaviour
    {
        public event Action<int> onHealthPointRestored;
        public event Action<int> onHealthPointChanged;

        private int _healthPoint;

        internal void Init(int healthPoint)
        {
            _healthPoint = healthPoint;
            onHealthPointRestored?.Invoke(_healthPoint);
        }

        internal void Attack(int attackPoint)
        {
            _healthPoint -= attackPoint;
            if (_healthPoint < 0)
            {
                _healthPoint = 0;
            }

            onHealthPointChanged?.Invoke(_healthPoint);
        }
    }
}
