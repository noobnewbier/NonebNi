using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using UnityEngine;

namespace NonebNi.Core.Effects
{
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
            protected override EffectResult OnEvaluate(DamageEffect effect, EffectContext context)
            {
                return new (FindSequences());

                IEnumerable<ISequence> FindSequences()
                {
                    foreach (var damageReceiver in FindDamageReceivers(context))
                    {
                        var damageAmount = GetDamage(effect, context.ActionCaster, damageReceiver);
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

            private IEnumerable<UnitData> FindDamageReceivers(EffectContext context)
            {
                return context.TargetGroups.SelectMany(g => g.Targets).OfType<UnitData>();
            }

            private int GetDamage(DamageEffect effect, EntityData actionCaster, UnitData damageReceiver)
            {
                return effect._damages
                    .Select(d => d.CalculateDamage(actionCaster, damageReceiver))
                    .Sum();
            }

            public IEnumerable<(float damage, bool isKill, UnitData damageReceiver)> GetEffectPreview(DamageEffect effect, EffectContext context)
            {
                foreach (var damageReceiver in FindDamageReceivers(context))
                {
                    var damageAmount = GetDamage(effect, context.ActionCaster, damageReceiver);
                    var isKill = damageReceiver.Health > damageAmount;
                    yield return (damageAmount, isKill, damageReceiver);
                }
            }
        }
    }
}