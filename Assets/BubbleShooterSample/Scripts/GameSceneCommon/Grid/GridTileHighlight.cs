using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GridTileHighlight : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> _spriteRendererList;

        public Transform CachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }
                return _cachedTransform;
            }
        }
        private Transform _cachedTransform;

        public void UpdateEditingToolType(GridEditingToolType toolType)
        {
            int toolIndex = (int)toolType;
            for (int i = 0; i < _spriteRendererList.Count; i++)
            {
                _spriteRendererList[i].gameObject.SetActive(i == toolIndex);
            }
        }
    }
}
