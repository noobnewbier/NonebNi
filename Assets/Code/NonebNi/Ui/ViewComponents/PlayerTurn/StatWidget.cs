using NonebNi.Core.Stats;
using TMPro;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class StatWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueText = null!;

        public void Show(int value)
        {
            valueText.text = value.ToString();
        }

        public void Show(int value, int maxValue)
        {
            valueText.text = $"{value}/{maxValue}";
        }

        public void Show(Stat stat)
        {
            if (stat == Stat.Invalid) Log.Error("Trying to show invalid stat - something went wrong upstream");

            if (stat.HasMaxLimit)
                Show(stat.CurrentValue, stat.MaxValue);
            else
                Show(stat.CurrentValue);
        }
    }
}