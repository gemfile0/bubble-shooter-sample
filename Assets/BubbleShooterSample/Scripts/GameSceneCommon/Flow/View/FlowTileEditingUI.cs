using BubbleShooterSample.Helper.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterSample
{
    public class FlowTileEditingUI : MonoBehaviour
    {
        [SerializeField] private GameObject _dropdownPanelObject;
        [SerializeField] private TMP_Dropdown _flowTileTypeDropdown;
        [SerializeField] private List<Toggle> _toolToggleList;

        public event Action<FlowTileType> onFlowTileTypeChanged;
        public event Action<GridEditingToolType> onFlowEditingToolChanged;

        private void Awake()
        {
            _flowTileTypeDropdown.FillDropdownOptionValues<FlowTileType>(50, 20);
        }

        private void Start()
        {
            _dropdownPanelObject.SetActive(false);
        }

        public void SetActive()
        {
            _dropdownPanelObject.SetActive(true);
        }

        public void OnDropdownValueChanged(int index)
        {
            OnFlowTileTypeChanged();
        }

        public void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                int selectedIndex = -1;
                for (int i = 0; i < _toolToggleList.Count; i++)
                {
                    Toggle toggle = _toolToggleList[i];
                    if (toggle.isOn)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                onFlowEditingToolChanged?.Invoke((GridEditingToolType)selectedIndex);
            }
        }

        private void OnFlowTileTypeChanged()
        {
            onFlowTileTypeChanged?.Invoke((FlowTileType)_flowTileTypeDropdown.value);
        }

        internal void UpdateToolType(GridEditingToolType toolType)
        {
            int toolIndex = (int)toolType;
            _toolToggleList[toolIndex].isOn = true;
        }
    }
}
