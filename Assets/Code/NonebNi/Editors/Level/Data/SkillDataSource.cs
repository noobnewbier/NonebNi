using NonebNi.Core.Units.Skills;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level.Data
{
    [CreateAssetMenu(fileName = nameof(SkillDataSource), menuName = MenuName.Data + nameof(SkillData))]
    public class SkillDataSource : ScriptableObject
    {
        [SerializeField] private string skillName = null!;
        [SerializeField] private int coolDown;
        [SerializeField] private Sprite icon = null!;

        public SkillData CreateData() => new SkillData(skillName, coolDown, icon);
    }
}