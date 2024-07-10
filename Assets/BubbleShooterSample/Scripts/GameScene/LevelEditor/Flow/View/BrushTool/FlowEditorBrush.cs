using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelEditor
{
    public class FlowEditorBrush : MonoBehaviour
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

        public void UpdateEditingToolType(FlowEditorBrushToolType toolType)
        {
            int toolIndex = (int)toolType;
            for (int i = 0; i < _spriteRendererList.Count; i++)
            {
                _spriteRendererList[i].gameObject.SetActive(i == toolIndex);
            }
        }
    }
}
