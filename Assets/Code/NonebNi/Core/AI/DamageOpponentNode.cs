using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Effects;
using NonebNi.Core.FlowControl;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Units;
using Action = Unity.Behavior.Action;

namespace NonebNi.Core.AI
{
    //todo: perhaps this is a subgraph? each chap will have different actions afterall... we can share the same subgraph and say if you don't have that action set to -1 though.

    //todo: for any AI Agent, we must come up with formulas that take various values into account, see the embracing dark art + normalization GDC for it. Without the formula we have nothing
    //luckily though with some luck we can code one general AI and tweak modifer, that plus available action alone should do most of the job.

    /// <summary>
    /// This node tries to maximize damage it can deal. It doesn't care if it's a killing blow, just the max damage to everyone
    /// affected.
    /// If you want to take killing blow into account go combine with other nodes and use formula's tag to achieve this.
    /// </summary>
    public class DamageOpponentNode : Action, IUtilityNode
    {
        public async IAsyncEnumerable<(ActionCommand command, Utility utility)> FindActionAndUtility(UnitData controlledUnit)
        {
            var agentContext = this.GetContext();
            if (agentContext == null) yield break;

            var deps = await GameContext.Get<Dependencies>();

            //todo: in the future we want to be able to customize which action to use here.
            /*
             * Note:
             * - Perhaps instead of just calculating damage we should aggregate actions output so it consider more than damage... Return the move to the selector with their associated utility?
             * - We can do this later, for now just make it work. Besides it might make tweaking more difficult...?
             */

            // finding the damage we can deal on all options
            var actions = controlledUnit.Actions;
            var commandDamages = FindCommandAndDamages(controlledUnit, actions, deps);

            // normalize the damage against the highest damage we can deal, and return as utility - the divide by highest damage bit kind of threw me off...?
            var highestDamage = commandDamages.Values.Max();
            foreach (var (command, damage) in commandDamages)
            {
                var normalizedDamage = damage / highestDamage;
                yield return (command, ("total-damage", normalizedDamage));
            }
        }

        private static Dictionary<ActionCommand, float> FindCommandAndDamages(UnitData controlledUnit, NonebAction[] actions, Dependencies deps)
        {
            var actionDamage = new Dictionary<ActionCommand, float>();

            foreach (var action in actions)
            {
                var damageEffects = action.Effects.OfType<DamageEffect>().ToArray();

                if (!damageEffects.Any()) continue;

                foreach (var command in deps.OptionFinder.FindOptionsForActor(controlledUnit).OfType<ActionCommand>())
                {
                    //todo: think - is it better if we feed in tag externally, I don't know man.
                    var damageSum = 0f;
                    var effectContext = deps.ActionCommandEvaluator.FindEffectContext(command);

                    foreach (var effect in damageEffects)
                    foreach (var (damage, _, __) in deps.DamageEvaluator.GetEffectPreview(effect, effectContext))
                        damageSum += damage;

                    actionDamage[command] = damageSum;
                }
            }

            return actionDamage;
        }


        public record Dependencies(DamageEffect.Evaluator DamageEvaluator, IActionOptionFinder OptionFinder, IActionCommandEvaluator ActionCommandEvaluator);
    }
}