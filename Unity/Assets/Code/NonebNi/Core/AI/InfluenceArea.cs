using System.Collections.Generic;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.AI
{
    /// <summary>
    /// Concrete values we got from influence map - these are meant to be used for the duration of one action, and nothing
    /// more.
    /// </summary>
    public record InfluenceArea
    {
        private readonly Dictionary<Coordinate, float> _influences = new();

        public InfluenceArea(InfluenceArea origin)
        {
            _influences = new Dictionary<Coordinate, float>(origin._influences);
        }


        public float this[Coordinate key]
        {
            get => _influences.GetValueOrDefault(key);
            set => _influences[key] = value;
        }

        public static InfluenceArea operator +(InfluenceArea left, InfluenceArea right)
        {
            var toReturn = new InfluenceArea(left);

            foreach (var (key, value) in right._influences) toReturn[key] += value;

            return toReturn;
        }

        public static InfluenceArea operator -(InfluenceArea left, InfluenceArea right) => left + -right;

        public static InfluenceArea operator +(InfluenceArea operand) => operand;

        public static InfluenceArea operator -(InfluenceArea operand)
        {
            var toReturn = new InfluenceArea(operand);

            foreach (var (key, value) in operand._influences) toReturn[key] = -value;

            return toReturn;
        }

        public static InfluenceArea operator *(InfluenceArea left, float factor)
        {
            var toReturn = new InfluenceArea(left);

            foreach (var key in left._influences.Keys) toReturn[key] *= factor;

            return toReturn;
        }

        public static InfluenceArea operator *(InfluenceArea left, InfluenceArea right)
        {
            var toReturn = new InfluenceArea(left);

            foreach (var (key, value) in right._influences) toReturn[key] *= value;

            return toReturn;
        }

        /*
         * Note:
         * Haven't implemented division - divide by zero is a bit tricky to handle gracefully
         */
    }
}