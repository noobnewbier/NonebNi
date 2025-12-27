using System.Collections;
using System.Collections.Generic;
using NonebNi.Core.Actions;
using NonebNi.Core.Effects;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.EditModeTests.GameLogic;

namespace NonebNi.EditModeTests
{
    public static class TestHelpers
    {
        public static void EvaluateEnumerable(this IEnumerable enumerable)
        {
            foreach (var _ in enumerable) { }
        }

        //Just a helper func that allows us to use the params modifier to make the test slightly cleaner to read.
        public static IEnumerable<ISequence> Evaluate(
            this IEffectEvaluator evaluator,
            Effect effect,
            IMap map,
            EntityData actionCaster,
            params IActionTarget[] targets
        )
        {
            return evaluator
                   .Evaluate(effect, new (map, new (TestData.Action, actionCaster), new[] { new EffectTargetGroup(targets) }))
                   .result
                   .Sequences;
        }
    }
}