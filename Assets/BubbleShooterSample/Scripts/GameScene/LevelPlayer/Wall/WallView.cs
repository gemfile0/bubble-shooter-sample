using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class WallView : MonoBehaviour
    {
        [SerializeField] private Transform _leftWall;
        [SerializeField] private Transform _rightWall;

        public void Init()
        {
            SetupWalls();
        }

        private void SetupWalls()
        {
            Camera mainCamera = Camera.main;
            Vector3 leftBottomBoundary = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 rightTopBoundary = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

            float screenWidth = rightTopBoundary.x - leftBottomBoundary.x;
            float screenHeight = rightTopBoundary.y - leftBottomBoundary.y;

            Vector3 wallSize = new Vector3(1f, screenHeight, 1f);
            _leftWall.transform.localScale = wallSize;
            _rightWall.transform.localScale = wallSize;

            float wallWidthHalf = wallSize.x / 2f;
            Vector3 mainCameraPosition = mainCamera.transform.position;
            _leftWall.position = new Vector3(leftBottomBoundary.x - wallWidthHalf, mainCameraPosition.y, 0);
            _rightWall.position = new Vector3(rightTopBoundary.x + wallWidthHalf, mainCameraPosition.y, 0);
        }
    }
}
