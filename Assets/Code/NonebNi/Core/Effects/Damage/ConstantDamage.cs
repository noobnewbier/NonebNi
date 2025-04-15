using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public class ConstantDamage : Damage
    {
        [SerializeField] private int amount;

        public ConstantDamage(int amount)
        {
            this.amount = amount;
        }

        public override int CalculateDamage(EntityData actionCaster, EntityData _) => amount;
    }
}