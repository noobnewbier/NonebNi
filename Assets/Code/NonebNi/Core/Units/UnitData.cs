using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Units.Skills;
using UnityEngine;

namespace NonebNi.Core.Units
{
    [Serializable]
    public class UnitData : EntityData
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int health;
        [SerializeField] private Sprite icon;
        [SerializeField] private SkillData[] skillDatas;

        public UnitData(string name,
                        int maxHealth,
                        int health,
                        Sprite icon,
                        SkillData[] skillDatas) : base(name)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.icon = icon;
            this.skillDatas = skillDatas;
        }

        public SkillData[] SkillDatas => skillDatas;

        public Sprite Icon => icon;

        public int MaxHealth => maxHealth;
        public int Health => health;
    }
}