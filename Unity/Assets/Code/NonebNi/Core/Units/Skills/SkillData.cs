using System;
using UnityEngine;

namespace NonebNi.Core.Units.Skills
{
    [Serializable, Obsolete("Replaced by action")]
    //TODO: maybe not - make it ui only? sounds like a horrible idea let's not do that.
    public class SkillData
    {
        [field: SerializeField] public string ActionId { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public SkillData(string actionId, Sprite icon)
        {
            ActionId = actionId;
            Icon = icon;
        }
    }
}