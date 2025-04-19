using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class WeaponBasedRange : Range
    {
        public override int CalculateRange(EntityData actionCaster)
        {
            if (actionCaster is not UnitData unitData)
            {
                Log.Error("Unexpected argument, only unit is expected to have equipment!");
                return 0;
            }

            return unitData.WeaponRange;
        }
    }
}