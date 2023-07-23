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
        [SerializeField] private int speed;
        [Range(0, 100)] [SerializeField] private int initiative;


        public UnitData(
            Guid guid,
            string name,
            string factionId,
            int maxHealth,
            int health,
            Sprite icon,
            SkillData[] skillDatas,
            int initiative,
            int speed) : base(name, guid, factionId)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.icon = icon;
            this.skillDatas = skillDatas;
            this.initiative = initiative;
            this.speed = speed;
        }

        public UnitData(UnitData unitData) : this(
            System.Guid.NewGuid(),
            unitData.Name,
            unitData.FactionId,
            unitData.maxHealth,
            unitData.health,
            unitData.icon,
            unitData.skillDatas,
            unitData.initiative,
            unitData.speed
        ) { }

        public int Speed => speed;

        public int Initiative => initiative;

        public SkillData[] SkillDatas => skillDatas;
        public Sprite Icon => icon;
        public int MaxHealth => maxHealth;

        public int Health
        {
            get => health;
            set => health = Mathf.Clamp(value, 0, maxHealth);
        }

        public UnitData WithSpeed(int newSpeed)
        {
            var newData = new UnitData(this)
            {
                speed = newSpeed
            };

            return newData;
        }
    }
}