using BubbleShooterSample.System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.GameData
{
    [Serializable]
    public struct SaveData
    {
        public LevelDataId ID;
        public string Data;
    }

    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public List<SaveData> SaveDataList { get; private set; } = new();
    }
}
