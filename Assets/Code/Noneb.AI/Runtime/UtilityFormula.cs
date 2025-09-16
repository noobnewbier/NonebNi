using System.Collections.Generic;
using System.Linq;
using Unity.Logging;

namespace Noneb.AI.Runtime
{
    public class UtilityFormula
    {
        private readonly Dictionary<UtilityFormula, float> _childAndWeight;
        private readonly string _tag;

        public UtilityFormula(string tag, Dictionary<UtilityFormula, float> childAndWeight)
        {
            _tag = tag;
            _childAndWeight = childAndWeight;
        }

        public float Calculate(Utility utility)
        {
            if (!_childAndWeight.Any())
                //no children - this is the leaf, return it and let parent handle the weight.
                return utility[_tag];

            var scoreAndWeight = new List<(float score, float weight)>();
            foreach (var (formula, weight) in _childAndWeight)
            {
                if (weight == 0) continue;

                var score = formula.Calculate(utility);
                scoreAndWeight.Add((score, weight));
            }

            var totalWeight = scoreAndWeight.Select(i => i.weight).Sum();
            var toReturn = 0f;
            foreach (var (score, weight) in scoreAndWeight)
            {
                var weightedScore = score * (weight / totalWeight);
                toReturn += weightedScore;
            }

            if (utility[_tag] > 0)
            {
                Log.Error("This doesn't really make sense, the formula work by having the action giving context for the leaf node, and we reconstruct the weight up the tree. I don't know what is happening but I will try to play along here");
                toReturn += utility[_tag];
            }

            return toReturn;
        }
    }
}