using NonebNi.Core.Units.Skills;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.EditorComponent.Entities.Skills
{
    /// <summary>
    ///     Temporary implementation to fill in the gaps - to be removed
    /// </summary>
    [CreateAssetMenu(fileName = nameof(SkillDataSource), menuName = MenuName.Data + nameof(SkillData))]
    public class SkillDataSource : ScriptableObject
    {
        [SerializeField] private string skillName = null!;
        [SerializeField] private int coolDown;
        [SerializeField] private Sprite icon = null!;

        public SkillData CreateData() => new(skillName, coolDown, icon);
    }
}