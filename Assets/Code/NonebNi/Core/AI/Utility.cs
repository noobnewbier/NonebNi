using System.Collections.Generic;
using Noneb.Tags.Runtime;

namespace NonebNi.Core.AI
{
    public class Utility
    {
        public static readonly Utility Invalid = new ()
        {
            IsValid = false
        };
        private readonly Dictionary<NonebTag, float> _scores;

        private Utility(Utility clone)
        {
            _scores = new (clone.Scores);
        }

        public Utility()
        {
            _scores = new ();
        }

        private bool IsValid { get; init; } = true;

        public IReadOnlyDictionary<NonebTag, float> Scores => _scores;

        public float this[NonebTag key]
        {
            get
            {
                if (!IsValid)
                    // effectively turning an option to be so bad it would never be picked
                    return -1;

                return _scores.GetValueOrDefault(key);
            }
            private set => _scores[key] = value;
        }

        public static implicit operator Utility((NonebTag key, float value) tuple)
        {
            var toReturn = new Utility
            {
                _scores =
                {
                    [tuple.key] = tuple.value
                }
            };

            return toReturn;
        }

        public static Utility operator +(Utility left, Utility right)
        {
            var toReturn = new Utility(left);

            foreach (var (key, value) in right.Scores)
            {
                var leftValue = toReturn[key];
                toReturn[key] = leftValue + value;
            }

            return toReturn;
        }
    }
}