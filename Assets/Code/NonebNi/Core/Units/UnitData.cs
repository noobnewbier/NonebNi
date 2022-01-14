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
        [Range(0, 100)] [SerializeField] private int initiative;


        public UnitData(Guid guid,
                        string name,
                        int maxHealth,
                        int health,
                        Sprite icon,
                        SkillData[] skillDatas,
                        int initiative) : base(name, guid)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.icon = icon;
            this.skillDatas = skillDatas;
            this.initiative = initiative;
        }

        public int Initiative => initiative;

        public SkillData[] SkillDatas => skillDatas;
        public Sprite Icon => icon;
        public int MaxHealth => maxHealth;

        public int Health
        {
            get => health;
            set => health = Mathf.Clamp(value, 0, maxHealth);
        }
    }
}