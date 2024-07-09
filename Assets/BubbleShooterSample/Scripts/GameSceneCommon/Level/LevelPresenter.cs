using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class LevelPresenter : MonoBehaviour, ILevelDataManagerSetter
    {
        [SerializeField] private LevelUI _levelUI;

        public ILevelDataManager LevelDataManager { private get; set; }

        private void Start()
        {
            _levelUI.UpdateLevelText(LevelDataManager.CurrentLevel);
        }
    }
}
