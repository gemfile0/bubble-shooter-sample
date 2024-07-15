using System;
using TMPro;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubbleShooterEditorUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        public event Action<int> onValueChanged;
        public event Action<int> onButtonClick;

        private string _latestCountStr;

        internal void UpdateUI(int value)
        {
            _latestCountStr = value.ToString();

            UpdateCountText();
        }

        public void OnValueChanged(string valueStr)
        {
            if (int.TryParse(valueStr, out int value))
            {
                onValueChanged?.Invoke(value);
            }
        }

        public void OnButtonClick(int valueOffset)
        {
            onButtonClick?.Invoke(valueOffset);
        }

        private void UpdateCountText()
        {
            if (_inputField.text != _latestCountStr)
            {
                _inputField.SetTextWithoutNotify(_latestCountStr);
            }
        }

        internal void RevertUI()
        {
            UpdateCountText();
        }
    }
}
