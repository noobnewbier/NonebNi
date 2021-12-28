using UnityEngine;

namespace NonebNi.Ui.Statistics.Unit
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform hasHealthArea;
        [SerializeField] private RectTransform fullHealthArea;

        public void Show(int currentHealth, int maxHealth)
        {
        }
    }
}