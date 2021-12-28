using NonebNi.Core.Units.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Statistics.Skill
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Image image = null!;
        [SerializeField] private TextMeshProUGUI cdTextMesh = null!;

        public void Show(SkillData dataSource)
        {
            image.sprite = dataSource.Icon;
            cdTextMesh.text = dataSource.Cooldown.ToString();
        }
    }
}