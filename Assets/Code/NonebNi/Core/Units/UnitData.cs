using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Stats;
using UnityEngine;

namespace NonebNi.Core.Units
{
    [Serializable]
    public class UnitData : EntityData
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private NonebAction[] actions;
        [field: SerializeField] public StatsCollection Stats { get; private set; } = new();

        public UnitData(string name) : this(
            System.Guid.NewGuid(),
            Array.Empty<NonebAction>(),
            Sprite.Create(Texture2D.redTexture, new Rect(0, 0, 1, 1), Vector2.zero),
            name,
            "debug-faction",
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ) { }

        public UnitData(
            Guid guid,
            IReadOnlyCollection<NonebAction> actions,
            Sprite icon,
            string name,
            string factionId,
            int maxHealth,
            int health,
            int initiative,
            int speed,
            int focus,
            int strength,
            int armor,
            int weaponRange,
            int fatigue,
            int maxFatigue) : base(name, guid, factionId)
        {
            this.icon = icon;
            this.actions = actions.ToArray();

            Stats.CreateStat("health", health, 0, maxHealth);
            Stats.CreateStat("initiative", initiative, 0, 100);
            Stats.CreateStat("speed", speed, 0, speed);
            Stats.CreateStat("focus", focus);
            Stats.CreateStat("strength", strength);
            Stats.CreateStat("armor", armor);
            Stats.CreateStat("weaponRange", weaponRange);
            Stats.CreateStat("fatigue", fatigue, 0, maxFatigue);
        }

        public UnitData(UnitData unitData) : this(
            System.Guid.NewGuid(),
            unitData.actions,
            unitData.icon,
            unitData.Name,
            unitData.FactionId,
            unitData.MaxHealth,
            unitData.Health,
            unitData.Initiative,
            unitData.Speed,
            unitData.Focus,
            unitData.Strength,
            unitData.Armor,
            unitData.WeaponRange,
            unitData.Fatigue,
            unitData.MaxFatigue
        ) { }

        public int Focus
        {
            get
            {
                var (_, value) = Stats.GetMaxValue("focus");

                return value;
            }
            private set => Stats.SetValue("focus", value);
        }

        public int Strength
        {
            get
            {
                var (_, value) = Stats.GetMaxValue("strength");

                return value;
            }
            private set => Stats.SetValue("strength", value);
        }

        public int Armor
        {
            get
            {
                var (_, value) = Stats.GetMaxValue("armor");

                return value;
            }
            private set => Stats.SetValue("armor", value);
        }

        //TODO: flesh out equipment design: https://www.notion.so/Equipment-02619835e80f4791b7702df4813cce24?pvs=4
        public int WeaponRange
        {
            get
            {
                var (_, value) = Stats.GetValue("weaponRange");

                return value;
            }
            private set => Stats.SetValue("weaponRange", value);
        }

        public int Speed
        {
            get
            {
                var (_, value) = Stats.GetValue("speed");
                return value;
            }
            set => _ = Stats.SetValue("speed", value);
        }

        public int Initiative
        {
            get
            {
                var (_, value) = Stats.GetValue("initiative");
                return value;
            }
            set => _ = Stats.SetValue("initiative", value);
        }

        public NonebAction[] Actions => actions;
        public Sprite Icon => icon;

        public int MaxHealth
        {
            get
            {
                var (_, value) = Stats.GetMaxValue("health");

                return value;
            }

            set => Stats.SetMaxValue("health", value);
        }

        public int Health
        {
            get
            {
                var (_, value) = Stats.GetValue("health");

                return value;
            }
            set => _ = Stats.SetValue("health", value);
        }

        public int MaxFatigue
        {
            get
            {
                var (_, value) = Stats.GetMaxValue("fatigue");

                return value;
            }
        }

        public int Fatigue
        {
            get
            {
                var (_, value) = Stats.GetValue("fatigue");

                return value;
            }
            set => _ = Stats.SetValue("fatigue", value);
        }

        public override bool IsTileOccupier => true;

        public void RestoreMovement()
        {
            var (_, maxSpeed) = Stats.GetMaxValue("speed");
            Speed = maxSpeed;
        }
    }
}