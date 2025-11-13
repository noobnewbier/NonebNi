using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Noneb.Tags.Runtime;

namespace NonebNi.Core.AI
{
    [DebuggerDisplay("{ToString(),nq}")]
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

            /*
             * Note:
             * NaN is treated as 0, it's okay most of the time, as we can get a NaN when:
             * - 0/0 -> in which case treating it as nothing is a safe bet
             * - 0 mult/div with infinity -> which we can still treat it as zero
             * - any num +-NaN -> in which case we want to take whatever the non-NaN amount and work with it.
             *
             * So it's really just more of an empirically okay kind of situation, I can regret later if me not logging a warning is a wrong decision
             */
            private set => _scores[key] = float.IsNaN(value) ?
                0 :
                value;
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

        public override string ToString()
        {
            return string.Join(",", _scores.Select(kv => $"{kv.Key}: {kv.Value}"));
        }
    }
}