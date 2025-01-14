using System;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Units
{
    [Serializable]
    public class UnitData : EntityData //TODO: this a record?
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int health;
        [SerializeField] private int maxFatigue;
        [SerializeField] private int fatigue;
        [SerializeField] private Sprite icon;
        [SerializeField] private NonebAction[] actions;
        [SerializeField] private int speed;
        [field: SerializeField] public int Focus { get; private set; }
        [field: SerializeField] public int Strength { get; private set; }
        [field: SerializeField] public int Armor { get; private set; }

        //TODO: flesh out equipment design: https://www.notion.so/Equipment-02619835e80f4791b7702df4813cce24?pvs=4
        [field: SerializeField] public int WeaponRange { get; private set; }


        [Range(0, 100), SerializeField] private int initiative;

        public UnitData(
            Guid guid,
            string name,
            string factionId,
            int maxHealth,
            int health,
            Sprite icon,
            NonebAction[] actions,
            int initiative,
            int speed,
            int focus,
            int strength,
            int armor,
            int weaponRange) : base(name, guid, factionId)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.icon = icon;
            this.actions = actions;
            this.initiative = initiative;
            this.speed = speed;
            Focus = focus;
            Strength = strength;
            Armor = armor;
            WeaponRange = weaponRange;
        }

        public UnitData(UnitData unitData) : this(
            System.Guid.NewGuid(),
            unitData.Name,
            unitData.FactionId,
            unitData.maxHealth,
            unitData.health,
            unitData.icon,
            unitData.actions,
            unitData.initiative,
            unitData.speed,
            unitData.Focus,
            unitData.Strength,
            unitData.Armor,
            unitData.WeaponRange
        ) { }

        public int Speed => speed;

        public int Initiative => initiative;

        public NonebAction[] Actions => actions;
        public Sprite Icon => icon;
        public int MaxHealth => maxHealth;

        public int Health
        {
            get => health;
            set => health = Mathf.Clamp(value, 0, maxHealth);
        }

        public int MaxFatigue => maxFatigue;

        public int Fatigue
        {
            get => fatigue;
            set => fatigue = Mathf.Clamp(value, 0, maxFatigue);
        }

        public override bool IsTileOccupier => true;

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