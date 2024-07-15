using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BaseLevelScene : MonoBehaviour, ILevelDataManagerSetter
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] protected GridPresenter _gridPresenter;

        protected Vector2 _totalGridSize;

        public ILevelDataManager LevelDataManager { private get; set; }

        protected virtual void Start()
        {
            _gridPresenter.Init();
            _totalGridSize = _gridPresenter.GetTotalGridSize();

            SetCameraPosition();
            LevelDataManager.RestoreLevelData();
        }

        private void SetCameraPosition()
        {
            float cameraX = _totalGridSize.x * 0.5f - _gridPresenter.HorizontalSpacing;
            float cameraY = _totalGridSize.y * 0.25f;
            Vector3 mainCameraPosition = new Vector3(cameraX, cameraY, -10f);
            _mainCamera.transform.position = mainCameraPosition;
        }
    }
}
