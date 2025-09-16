using System.Collections.Generic;

namespace Noneb.AI.Runtime
{
    public class Utility
    {
        private readonly Dictionary<string, float> _scores;

        private Utility(Utility clone)
        {
            _scores = new Dictionary<string, float>(clone.Scores);
        }

        public Utility()
        {
            _scores = new Dictionary<string, float>();
        }

        public IReadOnlyDictionary<string, float> Scores => _scores;


        public float this[string key]
        {
            get => _scores.GetValueOrDefault(key);
            set => _scores[key] = value;
        }

        public static implicit operator Utility((string key, float value) tuple)
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