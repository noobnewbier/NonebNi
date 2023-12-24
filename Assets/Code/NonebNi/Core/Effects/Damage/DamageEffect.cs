using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;

namespace NonebNi.Core.Effects
{
    //TODO: implement all those effecto
    public class DamageEffect : Effect
    {
        //https://www.notion.so/Action-System-eda1779accf74f97906f1cf9047f9506?pvs=4
        private readonly Damage[] _damages;

        public DamageEffect(params Damage[] damages)
        {
            _damages = damages;
        }

        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            foreach (var target in targets)
            {
                if (target is not UnitData damageReceiver) continue;

                var damageAmount = _damages
                    .Select(d => d.CalculateDamage(actionCaster, damageReceiver))
                    .Sum();
                damageReceiver.Health -= damageAmount;

                if (damageReceiver.Health <= 0)
                {
                    if (!map.Remove(damageReceiver))
                        throw new InvalidOperationException(
                            "Shouldn't be able to evaluate command with targets that's ain't even on the map"
                        );

                    yield return new DieSequence(damageReceiver);
                }
                else
                {
                    yield return new DamageSequence(actionCaster, damageReceiver, damageAmount);
                }
            }
        }
    }
}