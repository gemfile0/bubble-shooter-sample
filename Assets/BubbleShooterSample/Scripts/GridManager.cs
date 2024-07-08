using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid _gridPrefab;
    [SerializeField] private int _rows = 8;
    [SerializeField] private int _columns = 10;
    [SerializeField] private float _gridRadius = 0.5f;

    private List<Vector2> _gridPositions = new List<Vector2>();

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        float horizontalSpacing = Mathf.Sqrt(3) * _gridRadius;
        float horizontalSpacingHalf = horizontalSpacing / 2f;
        float verticalSpacing = 1.5f * _gridRadius;

        for (int row = 0; row < _rows; row++)
        {
            int colsInRow = (row % 2 == 0) ? _columns : _columns - 1; // 짝수 행은 한 개 적게
            for (int col = 0; col < colsInRow; col++)
            {
                // 홀수 행일 경우, x 좌표를 오른쪽으로 반 칸 이동
                float xOffset = (row % 2 != 0) ? horizontalSpacingHalf : 0;
                float xPos = col * horizontalSpacing + xOffset;
                float yPos = row * verticalSpacing;

                Vector2 position = new Vector2(xPos, yPos);
                _gridPositions.Add(position);

                Grid grid = Instantiate(_gridPrefab, position, Quaternion.identity, transform);
                grid.Init(new Vector2(col, row));
            }
        }
    }

    public List<Vector2> GetGridPositions()
    {
        return _gridPositions;
    }
}