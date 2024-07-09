using System.Text;
using TMPro;
using UnityEngine;

namespace BubbleShooterSample
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;

        private StringBuilder _levelTextBuilder;

        private void Awake()
        {
            _levelTextBuilder = new StringBuilder();
        }

        internal void UpdateLevelText(int currentLevel)
        {
            _levelText.text = BuildLevelText(currentLevel);
        }

        private string BuildLevelText(int currentLevel)
        {
            _levelTextBuilder.Length = 0;
            _levelTextBuilder.Append("Level: ");
            _levelTextBuilder.Append(currentLevel);
            return _levelTextBuilder.ToString();
        }
    }
}
