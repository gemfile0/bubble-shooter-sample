
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private TextMeshPro _indexText;

    private Vector2 _index;

    public void Init(Vector2 index)
    {
        _index = index;

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        _indexText.text = $"({_index.x}, {_index.y})";
    }
}