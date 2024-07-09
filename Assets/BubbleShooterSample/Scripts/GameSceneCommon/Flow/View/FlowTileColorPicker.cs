using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterSample
{
    public class FlowTileColorPicker : MonoBehaviour
    {
        [SerializeField] private GameObject _colorPickerPaletteObject;
        [SerializeField] private List<Toggle> _colorPickerToggleList;
        [SerializeField] private List<Color> _colorPickerColorList;
        [SerializeField] private Image pickedColorImage;

        public Color PickedColor => pickedColorImage.color;

        public event Action<Color> onColorPicked;

        private void Start()
        {
            _colorPickerPaletteObject.SetActive(false);
        }

        public void OnPickedColorClick()
        {
            _colorPickerPaletteObject.SetActive(true);

            int selectedIndex = _colorPickerColorList.FindIndex(color => color == pickedColorImage.color);
            if (selectedIndex != -1)
            {
                Toggle pickedToggle = _colorPickerToggleList[selectedIndex];
                pickedToggle.SetIsOnWithoutNotify(true);
            }
            else
            {
                Debug.LogWarning($"인덱스를 찾지 못했습니다.");
            }
        }

        public void OnToggleValueChanged(bool value)
        {
            _colorPickerPaletteObject.SetActive(false);

            if (value)
            {
                int selectedIndex = _colorPickerToggleList.FindIndex(toggle => toggle.isOn);
                if (selectedIndex != -1)
                {
                    Color pickedColor = _colorPickerColorList[selectedIndex];
                    pickedColorImage.color = pickedColor;
                    onColorPicked?.Invoke(pickedColor);
                }
                else
                {
                    Debug.LogWarning($"인덱스를 찾지 못했습니다.");
                }
            }
        }
    }
}
