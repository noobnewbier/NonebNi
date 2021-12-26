using System;
using NonebNi.Game.Entities;
using UnityEngine;

namespace NonebNi.Game.Units
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