using BubbleShooterSample.LevelEditor;
using System;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubbleShooterModel : MonoBehaviour
    {
        public event Action<int> onBubbleCountRestored;
        public event Action<int> onBubbleCountConsumed;

        public int BubbleCount => _bubbleCount;
        private int _bubbleCount;

        internal void RestoreLevelData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                BubbleShooterSaveData saveData = JsonUtility.FromJson<BubbleShooterSaveData>(dataStr);
                _bubbleCount = saveData.bubbleCount;

                onBubbleCountRestored?.Invoke(_bubbleCount);
            }
        }

        public bool ConsumeBubbleCount()
        {
            bool result = _bubbleCount > 0;
            if (result)
            {
                _bubbleCount -= 1;
                onBubbleCountConsumed?.Invoke(_bubbleCount);
            }

            return result;
        }
    }
}
