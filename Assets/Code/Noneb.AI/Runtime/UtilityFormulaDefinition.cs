using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Logging;
using Unity.Properties;
using UnityEngine;
using Composite = Unity.Behavior.Composite;

namespace Noneb.AI.Runtime
{
    /// <summary>
    /// The sole reason that this is not in GraphToolkit is that I am too lazy.
    /// This will NEVER run any ordinary AI behaviour. The purpose of this node is to let us use tree structure to define a
    /// formula similar to the math dark art GDC talk.
    /// In the ideal world I would write my own graph toolkit. But for now, this is good enough.
    /// </summary>
    [Serializable, GeneratePropertyBag, NodeDescription("UtilityFormulaDefinition", story: "Context [Tag] takes [Weight]", category: "Flow", id: "d521a8a2a717daac7857fd5f309e9624")]
    public partial class UtilityFormulaDefinition : Composite
    {
        //todo: handle negative 1 -> where the choice should never be taken. or just a way to flag invalid choice.
        //Todo: something to flag that the tag isn't used by the code.
        [field: SerializeReference] public BlackboardVariable<string> Tag { get; private set; } = new();
        [field: SerializeReference] public BlackboardVariable<float> Weight { get; private set; } = new();

        public UtilityFormula CreateFormula()
        {
            var childrenAndWeight = new Dictionary<UtilityFormula, float>();
            foreach (var child in Children)
            {
                if (child is not UtilityFormulaDefinition definition)
                {
                    Log.Error($"What are you doing here {child.GetType()}, it's a formula in tree not an AI graph!");
                    continue;
                }

                // Weight must be greater than 0, we don't need to handle negative weight in the formula.
                if (definition.Weight <= 0) continue;

                var childFormula = definition.CreateFormula();
                childrenAndWeight[childFormula] = definition.Weight;
            }

            var formula = new UtilityFormula(Tag, childrenAndWeight);
            return formula;
        }


        protected override Status OnStart()
        {
            Log.Error("Should've never got here");
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Log.Error("Should've never got here");
            return Status.Success;
        }

        protected override void OnEnd()
        {
            Log.Error("Should've never got here");
        }
    }
}