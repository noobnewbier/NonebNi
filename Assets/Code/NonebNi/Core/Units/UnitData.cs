using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Units
{
    [Serializable]
    public class UnitData : EntityData
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float health;

        public UnitData(string name, float maxHealth, float health) : base(name)
        {
            this.maxHealth = maxHealth;
            this.health = health;
        }

        public float MaxHealth => maxHealth;
        public float Health => health;
    }
}