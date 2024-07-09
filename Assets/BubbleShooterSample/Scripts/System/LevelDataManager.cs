using BubbleShooterSample.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BubbleShooterSample.System
{
    public interface ILevelRestorable
    {
        LevelDataId RestoreLevelID { get; }
        void RestoreLevelData(string dataStr);
    }

    public interface ILevelSavable : ILevelRestorable
    {
        string SaveLevelData();
    }

    public interface ILevelDataManagerSetter
    {
        ILevelDataManager LevelDataManager { set; }
    }

    public interface ILevelDataManager
    {
        int CurrentLevel { get; }
        void RestoreLevelData(Action<bool> onRestoringComplete = null);
        void SaveLevelData();
        void SaveSpecificLevelData(LevelDataId levelDataId);
    }

    public class LevelDataManager : MonoBehaviour,
                                    IGameObjectFinderSetter,
                                    ILevelDataManager
    {
        [Header("Data")]
        [SerializeField] private LevelFileData _levelFileData;

        public int CurrentLevel => _currentLevel;
        private int _currentLevel;

        private IEnumerable<ILevelRestorable> _levelRestorableList;
        private IEnumerable<ILevelSavable> _levelSavableList;
        private Action<bool> _onRestoringComplete;

        public void OnGameObjectFinderAwake(IGameObjectFinder finder)
        {
            Debug.Log("LevelDataManager.OnGameObjectFinderAwake");
            _levelRestorableList = finder.FindGameObjectOfType<ILevelRestorable>();
            _levelSavableList = _levelRestorableList.OfType<ILevelSavable>();

            foreach (var popupOpener in finder.FindGameObjectOfType<ILevelDataManagerSetter>())
            {
                popupOpener.LevelDataManager = this;
            }
        }

        private void Awake()
        {
            _currentLevel = 10;
        }

        public void SaveSpecificLevelData(LevelDataId levelDataId)
        {
            LevelData levelData = LoadLevelData();
            _SaveSpecificLevelData(levelData, levelDataId);
        }

        public void SaveLevelData()
        {
            //Debug.Log("SaveLevelDataToFile");
            LevelData levelData = LoadLevelData();
            _SaveWholeLevelData(levelData);
        }

        private LevelData LoadLevelData()
        {
            string levelDataPath = GetSaveLevelDataPath();
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);
            if (levelData == null)
            {
                levelData = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(levelData, levelDataPath);
            }

            return levelData;
        }

        public void RestoreLevelData(Action<bool> onRestoringComplete = null)
        {
            _onRestoringComplete = onRestoringComplete;

            _RestoreLevelData();
        }

        private void _RestoreLevelData()
        {
            string levelDataPath = GetSaveLevelDataPath();
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);
            if (levelData != null)
            {
                OnAssetLoaded(levelData);
            }
            else
            {
                Debug.LogWarning($"레벨 데이터가 존재하지 않습니다 : {levelDataPath}");
                _onRestoringComplete?.Invoke(false);
            }
        }

        private void OnAssetLoaded(LevelData levelData)
        {
            Debug.Log($"OnAssetLoaded: {levelData.name}");

            foreach (ILevelRestorable levelRestorable in _levelRestorableList)
            {
                string dataToLoad = null;
                if (levelData.SaveDataList != null && levelData.SaveDataList.Count > 0)
                {
                    dataToLoad = levelData.SaveDataList.FirstOrDefault(item => item.ID == levelRestorable.RestoreLevelID).Data;
                }
                levelRestorable.RestoreLevelData(dataToLoad);
            }

            _onRestoringComplete?.Invoke(true);
        }

        private void _SaveSpecificLevelData(LevelData levelData, LevelDataId levelDataId)
        {
            Debug.Log($"_SaveSpecificLevelData 1 : {levelDataId}");
            string levelDataStr = "";
            foreach (ILevelSavable savable in _levelSavableList)
            {
                if (savable.RestoreLevelID == levelDataId)
                {
                    levelDataStr = savable.SaveLevelData();
                    break;
                }
            }
            Debug.Log($"_SaveSpecificLevelData 2 : {levelDataStr}");

            if (string.IsNullOrEmpty(levelDataStr) == false)
            {
                bool dataExists = false;
                for (int i = 0; i < levelData.SaveDataList.Count; i++)
                {
                    if (levelData.SaveDataList[i].ID == levelDataId)
                    {
                        levelData.SaveDataList[i] = new SaveData
                        {
                            ID = levelDataId,
                            Data = levelDataStr
                        };
                        break;
                    }
                }

                if (dataExists == false)
                {
                    levelData.SaveDataList.Add(new SaveData
                    {
                        ID = levelDataId,
                        Data = levelDataStr
                    });
                }
            }

            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();
        }

        private void _SaveWholeLevelData(LevelData levelData)
        {
            //Debug.Log("_SaveLevelDataToFile");
            levelData.SaveDataList.Clear();
            foreach (ILevelSavable savable in _levelSavableList)
            {
                levelData.SaveDataList.Add(new SaveData
                {
                    ID = savable.RestoreLevelID,
                    Data = savable.SaveLevelData()
                });
            }
            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();
        }

        private string GetSaveLevelDataPath()
        {
            // e.g. {Assets/BubbleShooterSample/GameDatas}/{LevelData_1.asset}
            return $"{_levelFileData.GetLevelDataPath()}/{GetLevelDataFileName()}";
        }

        private string GetLevelDataFileName()
        {
            // e.g. {LevelData}_{1}.asset
            return $"{_levelFileData.LevelDataName}_{_currentLevel}.asset";
        }
    }
}
