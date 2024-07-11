using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BaseLevelScene : MonoBehaviour, ILevelDataManagerSetter
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] protected GridPresenter _gridPresenter;

        public ILevelDataManager LevelDataManager { private get; set; }

        protected virtual void Start()
        {
            _gridPresenter.Init();
            SetCameraPosition();
            LevelDataManager.RestoreLevelData();
        }

        private void SetCameraPosition()
        {
            float cameraX = (_gridPresenter.ColumnCount * _gridPresenter.HorizontalSpacing - _gridPresenter.HorizontalSpacing) / 2f;
            float cameraY = _gridPresenter.RowCount / 2 * _gridPresenter.VerticalSpacing / 2f;
            Vector3 mainCameraPosition = new Vector3(cameraX, cameraY, -10f);
            _mainCamera.transform.position = mainCameraPosition;
        }
    }
}
