using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class WallView : MonoBehaviour
    {
        [SerializeField] private Transform _leftWall;
        [SerializeField] private Transform _rightWall;

        private Vector2 _totalGridSize;

        public void Init(Vector2 totalGridSize)
        {
            _totalGridSize = totalGridSize;

            SetupWalls();
        }

        private void SetupWalls()
        {
            Camera mainCamera = Camera.main;
            Vector3 leftBottomBoundary = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 rightTopBoundary = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

            float screenHeight = rightTopBoundary.y - leftBottomBoundary.y;

            Vector3 wallSize = new Vector3(1f, screenHeight, 1f);
            _leftWall.transform.localScale = wallSize;
            _rightWall.transform.localScale = wallSize;

            Vector3 mainCameraPosition = mainCamera.transform.position;
            float gridWidthHalf = _totalGridSize.x * 0.5f;
            float wallWidthHalf = wallSize.x * 0.5f;

            float leftWallPositionX = mainCameraPosition.x - gridWidthHalf - wallWidthHalf;
            float rightWallPositionX = mainCameraPosition.x + gridWidthHalf + wallWidthHalf;

            _leftWall.position = new Vector3(leftWallPositionX, mainCameraPosition.y, 0);
            _rightWall.position = new Vector3(rightWallPositionX, mainCameraPosition.y, 0);
        }
    }
}
