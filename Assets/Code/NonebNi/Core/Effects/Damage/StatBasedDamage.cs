using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Effects
{
    public class StatBasedDamage : Damage
    {
        public enum StatType
        {
            Strength,
            Focus
        }

        private readonly float _ratio;
        private readonly StatType _statType;

        public StatBasedDamage(float ratio, StatType statType)
        {
            _ratio = ratio;
            _statType = statType;
        }

        public override int CalculateDamage(EntityData actionCaster, EntityData target)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Only unit has stats! Damage is always 0.");
                return 0;
            }

            var stat = _statType switch
            {
                StatType.Focus => unitData.Focus,
                StatType.Strength => unitData.Strength,
                _ => throw new ArgumentOutOfRangeException()
            };

            var rawDamage = stat * _ratio;
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