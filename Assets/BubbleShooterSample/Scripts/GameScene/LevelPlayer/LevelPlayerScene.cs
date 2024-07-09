using UnityEngine;

namespace BubbleShooterSample
{
    public class LevelPlayerScene : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GridManager _gridManager;

        private void Awake()
        {
            SetCameraPosition();
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
