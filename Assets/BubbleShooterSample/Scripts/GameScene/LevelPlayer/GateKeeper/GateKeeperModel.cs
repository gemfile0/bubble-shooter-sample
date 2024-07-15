using BubbleShooterSample.LevelEditor;
using System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperModel : MonoBehaviour
    {
        public event Action<int, int> onLevelDataRestored;
        public event Action<int> onHealthPointChanged;

        private int _skinIndex;
        private int _healthPoint;

        internal void RestoreLevelData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                GateKeeperSaveData saveData = JsonUtility.FromJson<GateKeeperSaveData>(dataStr);
                _skinIndex = saveData.skinIndex;
                _healthPoint = saveData.healthPoint;

                onLevelDataRestored?.Invoke(_skinIndex, _healthPoint);
            }
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
