using System;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    [Serializable]
    public class BubbleShooterSaveData
    {
        public int bubbleCount;
    }

    public class BubbleShooterEditorModel : MonoBehaviour
    {
        public event Action<int> onBubbleCountRestored;
        public event Action<int> onBubbleCountUpdated;

        public int BubbleCount => _bubbleCount;
        private int _bubbleCount;

        internal string SaveLevelData()
        {
            BubbleShooterSaveData data = new()
            {
                bubbleCount = _bubbleCount
            };
            return JsonUtility.ToJson(data);
        }

        internal void RestoreLevelData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                BubbleShooterSaveData saveData = JsonUtility.FromJson<BubbleShooterSaveData>(dataStr);
                _bubbleCount = saveData.bubbleCount;

                onBubbleCountRestored?.Invoke(_bubbleCount);
            }
        }

        internal void ChangeBubbleCount(int nextBubbleCount)
        {
            _bubbleCount = nextBubbleCount;
            onBubbleCountUpdated?.Invoke(_bubbleCount);
        }
    }
}
