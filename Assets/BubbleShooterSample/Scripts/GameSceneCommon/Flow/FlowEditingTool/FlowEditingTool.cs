using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class FlowEditingTool : MonoBehaviour
    {
        [SerializeField] private GridTileHighlight _highlightPrefab;
        [SerializeField] private GridView _gridView;
        [SerializeField] private FlowPresenter _flowPresenter;

        private GridTileHighlight _highlight;
        private IEnumerable<IGridTile> _gridTileList;
        private FlowEditingToolType _toolType;

        private void Start()
        {
            _gridTileList = _gridView.GridTileList;
            _highlight = Instantiate(_highlightPrefab, Vector3.zero, Quaternion.identity, transform);
            _flowPresenter.UpdateToolType(FlowEditingToolType.Add);
        }

        private void OnEnable()
        {
            _flowPresenter.onFlowEditingToolChanged += OnFlowEditingToolChanged;
        }

        private void OnDisable()
        {
            _flowPresenter.onFlowEditingToolChanged -= OnFlowEditingToolChanged;
        }

        private void OnFlowEditingToolChanged(FlowEditingToolType toolType)
        {
            _toolType = toolType;
            _highlight?.UpdateEditingToolType(_toolType);
        }

        private void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            IGridTile closestTileGrid = FindClosestGridTile(mousePosition);

            if (IsMouseInGrid(mousePosition))
            {
                _highlight.gameObject.SetActive(true);
                _highlight.CachedTransform.position = closestTileGrid.Position;

                if (Input.GetMouseButtonDown(0))
                {
                    if (_toolType == FlowEditingToolType.Add)
                    {
                        if (_flowPresenter.HasFlowTile(closestTileGrid.Index) == false)
                        {
                            _flowPresenter.CreateFlowTile(closestTileGrid.Index, closestTileGrid.Position, FlowTileType.Node);
                        }
                        _flowPresenter.SelectFlowTile(closestTileGrid.Index);
                    }
                    else if (_toolType == FlowEditingToolType.Remove)
                    {
                        if (_flowPresenter.HasFlowTile(closestTileGrid.Index))
                        {
                            _flowPresenter.RemoveFlowTile(closestTileGrid.Index);
                        }
                    }
                }
            }
            else
            {
                _highlight.gameObject.SetActive(false);
            }
        }

        private IGridTile FindClosestGridTile(Vector2 position)
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

        private bool IsMouseInGrid(Vector2 mousePosition)
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
