using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Logging;
using UnityEngine;
using UnityUtils.Constants;
using UnityUtils.Serialization;

namespace NonebNi.Core.AI
{
    /// <summary>
    /// The sole reason that this is not in GraphToolkit is that I am too lazy.
    /// This will NEVER run any ordinary AI behaviour. The purpose of this node is to let us use tree structure to define a
    /// formula similar to the math dark art GDC talk.
    /// In the ideal world I would write my own graph toolkit. But for now, this is good enough.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(UtilityFormula), menuName = MenuName.Data + nameof(UtilityFormula))]
    public class UtilityFormula : ScriptableObject
    {
        /*
         * A graph editor is still nice as it allow me to give comment in the structure.
         * For now though, this is good enough.
         */

        //todo: handle negative 1 -> where the choice should never be taken. or just a way to flag invalid choice.
        //Todo: something to flag that the tag isn't used by the code.
        [SerializeField] private FormulaImp definitionRoot = new ();

        public float Calculate(Utility utility) => definitionRoot.Calculate(utility);


        /// <summary>
        /// Exists to help me reason the recursive logic, perhaps not strictly necessary.
        /// </summary>
        [Serializable]
        private class FormulaImp
        {
            [SerializeField] private string tag = string.Empty;
            [SerializeField] private SerializableDictionary<FormulaImp, float> childAndWeight = new ();

            public float Calculate(Utility utility)
            {
                if (!childAndWeight.Any())
                    //no children - this is the leaf, return it and let parent handle the weight.
                    return utility[tag];

                var scoreAndWeight = new List<(float score, float weight)>();

                foreach (var (formula, weight) in childAndWeight)
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

                if (utility[tag] > 0)
                {
                    Log.Error("This doesn't really make sense, the formula work by having the action giving context for the leaf node, and we reconstruct the weight up the tree. I don't know what is happening but I will try to play along here");
                    toReturn += utility[tag];
                }

                return toReturn;
            }

            public override string ToString()
            {
                var label = string.IsNullOrWhiteSpace(tag) ?
                    "EMPTY" :
                    tag;
                var child = childAndWeight.Select(pair => $"{pair.Key.tag}_{pair.Value}");
                var childString = string.Join("|", child);
                return $"{label} - CHILD: {childString}";
            }
        }
    }
}