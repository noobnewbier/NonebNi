using NonebNi.Core.Units;
using NonebNi.Ui.Statistics.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Statistics.Unit
{
    public class UnitDetailStatView : MonoBehaviour
    {
        [SerializeField] private Transform skillPanelRoot;
        [SerializeField] private Image image;
        [SerializeField] private HealthBarView healthBarView;

        [SerializeField] private SkillView skillViewPrefab;

        public void Show(UnitData unitData)
        {
            image.sprite = unitData.Icon;
            healthBarView.Show(unitData.Health, unitData.MaxHealth);
            foreach (var data in unitData.SkillDatas)
            {
                var skillView = Instantiate(skillViewPrefab, skillPanelRoot);
                skillView.Show(data);
            }
        }
    }
}