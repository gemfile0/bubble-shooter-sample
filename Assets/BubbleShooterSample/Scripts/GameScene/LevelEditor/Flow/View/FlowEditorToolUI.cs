using BubbleShooterSample.Helper.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterSample.LevelEditor
{
    public class FlowEditorToolUI : MonoBehaviour
    {
        [Header("Color Picker")]
        [SerializeField] private FlowEditorToolColorPicker _flowTileColorPicker;

        [Header("Tool Toggle")]
        [SerializeField] private List<Toggle> _toolToggleList;

        [Header("Tile Type Dropdown")]
        [SerializeField] private GameObject _dropdownPanelObject;
        [SerializeField] private TMP_Dropdown _flowTileTypeDropdown;

        public Color PickedColor => _flowTileColorPicker.PickedColor;

        public event Action<Color> onColorPicked
        {
            add { _flowTileColorPicker.onColorPicked += value; }
            remove { _flowTileColorPicker.onColorPicked -= value; }
        }
        public event Action<FlowTileType> onFlowTileTypeChanged;
        public event Action<FlowEditorBrushToolType> onFlowEditingToolChanged;

        private void Awake()
        {
            _flowTileTypeDropdown.FillDropdownOptionValues<FlowTileType>(50, 20);
        }

        private void Start()
        {
            UpdateTileTypeObject(false);
        }

        private void OnColorPicked(Color color)
        {

        }

        public void OnDropdownValueChanged(int index)
        {
            OnFlowTileTypeChanged();
        }

        public void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                int selectedIndex = _toolToggleList.FindIndex(toggle => toggle.isOn);
                if (selectedIndex != -1)
                {
                    onFlowEditingToolChanged?.Invoke((FlowEditorBrushToolType)selectedIndex);
                }
                else
                {
                    Debug.LogWarning($"인덱스를 찾지 못했습니다.");
                }
            }
        }

        private void OnFlowTileTypeChanged()
        {
            onFlowTileTypeChanged?.Invoke((FlowTileType)_flowTileTypeDropdown.value);
        }

        internal void UpdateToolType(FlowEditorBrushToolType toolType)
        {
            int toolIndex = (int)toolType;
            _toolToggleList[toolIndex].isOn = true;
        }

        internal void UpdateTileType(FlowTileType tileType)
        {
            _flowTileTypeDropdown.SetValueWithoutNotify((int)tileType);
        }

        public void UpdateTileTypeObject(bool value)
        {
            _dropdownPanelObject.SetActive(value);
        }
    }
}
