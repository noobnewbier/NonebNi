using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    public class StatBasedRange : Range
    {
        public enum StatType
        {
            Focus,
            Speed
        }

        private readonly float _ratio;
        private readonly StatType _statType;

        public StatBasedRange(float ratio, StatType statType)
        {
            _ratio = ratio;
            _statType = statType;
        }

        public override int CalculateRange(EntityData actionCaster)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Only unit has stats! Range is always 0.");
                return 0;
            }

            var stat = _statType switch
            {
                StatType.Focus => unitData.Focus,
                StatType.Speed => unitData.Speed,
                _ => throw new ArgumentOutOfRangeException()
            };

            var range = stat * _ratio;
            return Mathf.RoundToInt(range);
        }
    }
}