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
        [SerializeField] private float ratio;
        [SerializeField] private string statId;

        public StatBasedDamage(float ratio, string statId)
        {
            this.ratio = ratio;
            this.statId = statId;
        }

        public override int CalculateDamage(EntityData actionCaster, EntityData target)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Only unit has stats! Damage is always 0.");
                return 0;
            }

            var (success, stat) = unitData.Stats.GetValue(statId);
            if (!success)
            {
                return 0;
            }

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