using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using UnityEngine;

namespace NonebNi.Core.AI
{
    /// <summary>
    /// Too lazy now -> just a simple linear falloff, in the future we need something that will take unit's characteristic into
    /// account.
    /// Perhaps something editable would be nice? But for playable prototype this is good enough.
    /// </summary>
    public record BasicInfluenceSource : IInfluenceSource
    {
        private readonly IReadOnlyMap _map;

        //note: wbn if we have a way to easily access the position... a wrapper?
        private readonly UnitData _unitData;

        public BasicInfluenceSource(UnitData unitData, IReadOnlyMap map)
        {
            _unitData = unitData;
            _map = map;
        }

        public float FindInfluence(Coordinate coord)
        {
            var unitPos = _map.Find(_unitData);

            var dist = unitPos.DistanceTo(coord);

            const int maxRange = 5;
            var distStrength = Mathf.Lerp(1, 0, (float)dist / maxRange);

            var healthStrength = (float)_unitData.Health / _unitData.MaxHealth;

            return distStrength * healthStrength;
        }

        /*
         * Not sure if the influence map should maintain itself(similar to how weak reference doesn't require any cleanup),
         * for now we will see if it works out, if not we can always ditch the idea.
         */
        public bool IsValid() => _unitData.Health > 0;
    }
}