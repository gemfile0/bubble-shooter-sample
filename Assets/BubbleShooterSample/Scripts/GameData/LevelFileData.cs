using UnityEngine;

namespace BubbleShooterSample.GameData
{
    [CreateAssetMenu(menuName = "Bubble Shooter Sample/Level File Data")]
    public class LevelFileData : ScriptableObject
    {
        [SerializeField] private string _levelDataPathBase = "Assets/BubbleShooterSample/GameDatas";
        [SerializeField] private string _levelDataName = "LevelData";

        public string GetLevelDataPath()
        {
            return $"{_levelDataPathBase}";
        }
        public string LevelDataName => _levelDataName;
    }
}
