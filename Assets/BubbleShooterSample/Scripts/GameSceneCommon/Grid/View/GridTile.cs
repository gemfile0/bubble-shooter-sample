
using TMPro;
using UnityEngine;

namespace BubbleShooterSample
{
    public interface IGridTile
    {
        Vector2Int Index { get; }
        Vector2 Position { get; }
    }

    public class GridTile : MonoBehaviour, IGridTile
    {
        [SerializeField] private TextMeshPro _indexText;

        public Vector2Int Index => _index;
        public Vector2 Position => _position;

        private Vector2Int _index;
        private Vector2 _position;

        public void Init(Vector2Int index, Vector2 position)
        {
            _index = index;
            _position = position;

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _indexText.text = $"({_index.x}, {_index.y})";
        }
    }
}