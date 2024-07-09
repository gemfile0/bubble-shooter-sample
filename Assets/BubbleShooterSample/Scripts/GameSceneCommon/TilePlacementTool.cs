using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class TilePlacementTool : MonoBehaviour
    {
        [SerializeField] private GameObject _highlightPrefab;
        [SerializeField] private GridView _gridView;
        [SerializeField] private FlowPresenter _flowPresenter;

        private Transform _highlightTransform;
        private GameObject _highlightObject;
        private IEnumerable<IGridTile> _gridTileList;

        void Start()
        {
            _gridTileList = _gridView.GridTileList;
            _highlightObject = Instantiate(_highlightPrefab, Vector3.zero, Quaternion.identity, transform);
            _highlightTransform = _highlightObject.transform;
            _highlightTransform.gameObject.SetActive(false);
        }

        void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 하이라이트 위치 업데이트
            IGridTile closestTileGrid = FindClosestGridTile(mousePosition);

            // 마우스가 그리드 안에 있는지 확인
            if (IsMouseInGrid(mousePosition))
            {
                _highlightObject.SetActive(true);
                _highlightTransform.position = closestTileGrid.Position;

                // 클릭시 FlowTile 배치
                if (Input.GetMouseButtonDown(0))
                {
                    _flowPresenter.CreateFlowTile(closestTileGrid.Index, closestTileGrid.Position, FlowTileType.Node);
                }
            }
            else
            {
                _highlightObject.SetActive(false);
            }
        }

        IGridTile FindClosestGridTile(Vector2 position)
        {
            IGridTile closestGridTile = null;
            float minDistance = float.MaxValue;

            foreach (IGridTile gridTile in _gridTileList)
            {
                float distance = Vector2.Distance(position, gridTile.Position);
                if (distance < minDistance)
                {
                    closestGridTile = gridTile;
                    minDistance = distance;
                }
            }

            return closestGridTile;
        }

        bool IsMouseInGrid(Vector2 mousePosition)
        {
            bool result = false;
            float horizontalSpacingHalf = _gridView.HorizontalSpacing / 2f;
            foreach (IGridTile gridTile in _gridTileList)
            {
                float distance = Vector2.Distance(mousePosition, gridTile.Position);
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
