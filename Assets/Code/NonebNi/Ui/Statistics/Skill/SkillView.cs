using NonebNi.Core.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Statistics.Skill
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Image image = null!;

        public void Show(NonebAction dataSource)
        {
            image.sprite = dataSource.Icon;
        }
    }
}