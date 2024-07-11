using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BaseLevelScene : MonoBehaviour, ILevelDataManagerSetter
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GridView _gridView;

        public ILevelDataManager LevelDataManager { private get; set; }

        protected virtual void Start()
        {
            SetCameraPosition();
            LevelDataManager.RestoreLevelData();
        }

        private void SetCameraPosition()
        {
            float cameraX = (_gridView.ColumnCount * _gridView.HorizontalSpacing - _gridView.HorizontalSpacing) / 2f;
            float cameraY = _gridView.RowCount / 2 * _gridView.VerticalSpacing / 2f;
            Vector3 mainCameraPosition = new Vector3(cameraX, cameraY, -10f);
            _mainCamera.transform.position = mainCameraPosition;
        }
    }
}
