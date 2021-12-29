using UnityEngine;

namespace NonebNi.Ui.Statistics.Unit
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform hasHealthArea = null!;
        [SerializeField] private RectTransform fullHealthArea = null!;

        public void Show(int currentHealth, int maxHealth)
        {
            var fullWidth = fullHealthArea.rect.width;
            var healthWidth = fullWidth * (currentHealth / (float)maxHealth);

            hasHealthArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthWidth);
        }
    }
}