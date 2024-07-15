using System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    [Serializable]
    public class GateKeeperSaveData
    {
        public int healthPoint;
        public int skinIndex;
    }

    public class GateKeeperEditorModel : MonoBehaviour
    {
        public event Action<int, int> onLevelDataRestored;
        public event Action<int> onHealthPointUpdated;
        public event Action<int> onSkinIndexUpdated;

        public int HealthPoint => _healthPoint;
        private int _healthPoint;
        public int SkinIndex => _skinIndex;
        private int _skinIndex;

        internal string SaveLevelData()
        {
            GateKeeperSaveData data = new()
            {
                healthPoint = _healthPoint,
                skinIndex = _skinIndex
            };
            return JsonUtility.ToJson(data);
        }

        internal void RestoreLevelData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                GateKeeperSaveData saveData = JsonUtility.FromJson<GateKeeperSaveData>(dataStr);
                _healthPoint = saveData.healthPoint;
                _skinIndex = saveData.skinIndex;

                onLevelDataRestored?.Invoke(_healthPoint, _skinIndex);
            }
        }

        internal void ChangeHealthPoint(int nextValue)
        {
            _healthPoint = nextValue;
            onHealthPointUpdated?.Invoke(_healthPoint);
        }

        internal void ChangeSkinIndex(int nextValue)
        {
            _skinIndex = nextValue;
            onSkinIndexUpdated?.Invoke(_skinIndex);
        }
    }
}
