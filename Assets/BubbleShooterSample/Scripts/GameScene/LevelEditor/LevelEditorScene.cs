using BubbleShooterSample.System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class LevelEditorScene : MonoBehaviour, ILevelDataManagerSetter
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GridView _gridManager;

        public ILevelDataManager LevelDataManager { private get; set; }

        private void Start()
        {
            SetCameraPosition();
            LevelDataManager.RestoreLevelData();
        }

        private void SetCameraPosition()
        {
            float cameraX = (_gridManager.ColumnCount * _gridManager.HorizontalSpacing - _gridManager.HorizontalSpacing) / 2f;
            float cameraY = _gridManager.RowCount / 2 * _gridManager.VerticalSpacing / 2f;
            Vector3 mainCameraPosition = new Vector3(cameraX, cameraY, -10f);
            _mainCamera.transform.position = mainCameraPosition;
        }
    }
}
