using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class StatBasedRange : Range
    {
        [SerializeField] private float ratio;
        [SerializeField] private string statId;

        public StatBasedRange(float ratio, string statId)
        {
            this.ratio = ratio;
            this.statId = statId;
        }

        public override int CalculateRange(EntityData actionCaster)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Only unit has stats! Range is always 0.");
                return 0;
            }

            var (success, stat) = unitData.Stats.GetValue(statId);
            if (!success)
            {
                return 0;
            }

            var range = stat * ratio;
            return Mathf.RoundToInt(range);
        }
    }
}