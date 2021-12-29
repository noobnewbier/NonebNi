using NonebNi.Ui.Statistics.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Statistics.Unit
{
    public class UnitDetailStat : MonoBehaviour
    {
        public Transform skillPanelRoot = null!;
        public Image image = null!;
        public HealthBarView healthBarView = null!;
        public SkillView skillViewPrefab = null!;

        public UnitDetailStatView statView = null!;

        public void Init()
        {
            statView.Init(
                skillPanelRoot,
                image,
                healthBarView,
                skillViewPrefab
            );
        }
    }
}