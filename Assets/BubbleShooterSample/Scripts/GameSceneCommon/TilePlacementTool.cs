using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class TilePlacementTool : MonoBehaviour
    {
        [SerializeField] private GameObject _highlightPrefab;
        [SerializeField] private GridView _gridView;
        [SerializeField] private FlowView _flowView;

        private Transform _highlightTransform;
        private GameObject _highlightObject;
        private List<Vector2> _gridPositions;

        void Start()
        {
            _gridPositions = _gridView.GetGridPositions();
            _highlightObject = Instantiate(_highlightPrefab, Vector3.zero, Quaternion.identity, transform);
            _highlightTransform = _highlightObject.transform;
            _highlightTransform.gameObject.SetActive(false);
        }

        void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 하이라이트 위치 업데이트
            Vector2 closestGridPosition = FindClosestGrid(mousePosition);

            // 마우스가 그리드 안에 있는지 확인
            if (IsMouseInGrid(mousePosition))
            {
                _highlightObject.SetActive(true);
                _highlightTransform.position = closestGridPosition;

                // 클릭시 FlowTile 배치
                if (Input.GetMouseButtonDown(0))
                {
                    _flowView.CreateFlowTile(closestGridPosition, FlowTileType.Node);
                }
            }
            else
            {
                _highlightObject.SetActive(false);
            }
        }

        Vector2 FindClosestGrid(Vector2 position)
        {
            Vector2 closest = _gridPositions[0];
            float minDistance = Vector2.Distance(position, closest);

            foreach (Vector2 gridPosition in _gridPositions)
            {
                float distance = Vector2.Distance(position, gridPosition);
                if (distance < minDistance)
                {
                    closest = gridPosition;
                    minDistance = distance;
                }
            }

            return closest;
        }

        bool IsMouseInGrid(Vector2 mousePosition)
        {
            bool result = false;
            float horizontalSpacingHalf = _gridView.HorizontalSpacing / 2f;
            foreach (Vector2 gridPosition in _gridPositions)
            {
                float distance = Vector2.Distance(mousePosition, gridPosition);
                if (distance < horizontalSpacingHalf)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
