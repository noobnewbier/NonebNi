using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using UnityEngine;

namespace NonebNi.Core.Effects
{
    //TODO: implement all those effecto
    [Serializable]
    public class DamageEffect : Effect
    {
        [SerializeField] private string _animId;

        //https://www.notion.so/Action-System-eda1779accf74f97906f1cf9047f9506?pvs=4
        [SerializeReference] private Damage[] _damages;

        public DamageEffect(string animId, params Damage[] damages)
        {
            _animId = animId;
            _damages = damages;
        }

        public class Evaluator : Evaluator<DamageEffect>
        {
            protected override IEnumerable<ISequence> OnEvaluate(
                DamageEffect effect,
                EffectContext context)
            {
                foreach (var target in context.TargetGroups.SelectMany(g => g.Targets))
                {
                    if (target is not UnitData damageReceiver) continue;

                    var damageAmount = effect._damages
                        .Select(d => d.CalculateDamage(context.ActionCaster, damageReceiver))
                        .Sum();
                    damageReceiver.Health -= damageAmount;

                    if (damageReceiver.Health <= 0)
                    {
                        if (!context.Map.Remove(damageReceiver))
                            throw new InvalidOperationException(
                                "Shouldn't be able to evaluate command with targets that's ain't even on the map"
                            );

                        yield return new DieSequence(damageReceiver);
                    }
                    else
                    {
                        yield return new DamageSequence(context.ActionCaster, damageReceiver, damageAmount, effect._animId);
                    }
                }
            }
        }
    }
}