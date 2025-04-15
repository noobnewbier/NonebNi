using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public class StatBasedDamage : Damage
    {
        public enum StatType
        {
            Strength,
            Focus
        }

        [SerializeField] private float ratio;
        [SerializeField] private StatType statType;

        public StatBasedDamage(float ratio, StatType statType)
        {
            this.ratio = ratio;
            this.statType = statType;
        }

        public override int CalculateDamage(EntityData actionCaster, EntityData target)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Only unit has stats! Damage is always 0.");
                return 0;
            }

            var stat = statType switch
            {
                StatType.Focus => unitData.Focus,
                StatType.Strength => unitData.Strength,
                _ => throw new ArgumentOutOfRangeException()
            };

            var rawDamage = stat * ratio;
            if (target is not UnitData targetUnit)
            {
                Log.Error(
                    $"Currently we aren't taking stuffs that isn't an unit but also have health into account! We probably need to restructure {nameof(UnitData)} and {nameof(EntityData)} for that to happen"
                );
                return Mathf.RoundToInt(rawDamage);
            }

            var damageAfterArmor = rawDamage - targetUnit.Armor;
            return Mathf.RoundToInt(damageAfterArmor);
        }
    }
}