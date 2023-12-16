using System;
using System.Collections.Generic;
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
        //TODO: probs need to take in multiplier + some consts instead? Need to do this asap otherwise code had to work on assumption(which they kind of have to anyway)
        //https://www.notion.so/Action-System-eda1779accf74f97906f1cf9047f9506?pvs=4
        //We can probs do this by creating a Damage class to formulate different type of damage (magic damage scale with magic, constant damage just go, etc)
        private readonly Damage _damage;

        public DamageEffect(Damage damage)
        {
            _damage = damage;
        }

        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            foreach (var target in targets)
            {
                if (target is not UnitData damageReceiver) continue;

                var damageAmount = _damage.CalculateDamage(damageReceiver);
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