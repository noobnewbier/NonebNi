using System;
using System.Collections.Generic;
using System.Linq;
using Noneb.Localization.Runtime;
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

        public UnitData(NonebLocString name) : this(
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
            0,
            1,
            1
        ) { }

        public UnitData(
            Guid guid,
            IReadOnlyCollection<NonebAction> actions,
            Sprite icon,
            NonebLocString name,
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
            int maxFatigue,
            int fatigueRecovery,
            int maxActionPoint) : base(name, guid, factionId)
        {
            this.icon = icon;
            this.actions = actions.ToArray();

            Stats.CreateStat(StatId.Health, health, 0, maxHealth);
            Stats.CreateStat(StatId.Initiative, initiative, 0, 100); //todo: technically initiative and timetoact is two different concept, is bulking them tgt easier to code?
            Stats.CreateStat(StatId.Speed, speed, 0, speed);
            Stats.CreateStat(StatId.Focus, focus);
            Stats.CreateStat(StatId.Strength, strength);
            Stats.CreateStat(StatId.Armor, armor);
            Stats.CreateStat(StatId.WeaponRange, weaponRange);
            //todo: we can call fatigue stamina(or just MP) to fix the "reverse ui display" problem
            Stats.CreateStat(StatId.Fatigue, maxFatigue, 0, maxFatigue);
            Stats.CreateStat(StatId.ActionPoint, maxActionPoint, 0, maxActionPoint);
            Stats.CreateStat(StatId.FatigueRecovery, fatigue);
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
            unitData.MaxFatigue,
            unitData.FatigueRecovery,
            unitData.MaxActionPoint
        ) { }

        public int Focus
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.Focus);

                return value;
            }
            private set => Stats.SetValue(StatId.Focus, value);
        }

        public int Strength
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.Strength);

                return value;
            }
            private set => Stats.SetValue(StatId.Strength, value);
        }

        public int Armor
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.Armor);

                return value;
            }
            private set => Stats.SetValue(StatId.Armor, value);
        }

        //TODO: flesh out equipment design: https://www.notion.so/Equipment-02619835e80f4791b7702df4813cce24?pvs=4
        public int WeaponRange
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.WeaponRange);

                return value;
            }
            private set => Stats.SetValue(StatId.WeaponRange, value);
        }

        public int Speed
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.Speed);
                return value;
            }
            set => _ = Stats.SetValue(StatId.Speed, value);
        }

        public int Initiative
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.Initiative);
                return value;
            }
            set => _ = Stats.SetValue(StatId.Initiative, value);
        }

        public NonebAction[] Actions => actions;
        public Sprite Icon => icon;

        public int MaxHealth
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.Health);

                return value;
            }

            set => Stats.SetMaxValue(StatId.Health, value);
        }

        public int Health
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.Health);

                return value;
            }
            set => _ = Stats.SetValue(StatId.Health, value);
        }

        public int MaxFatigue
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.Fatigue);

                return value;
            }
        }

        public int Fatigue
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.Fatigue);

                return value;
            }
            set => _ = Stats.SetValue(StatId.Fatigue, value);
        }

        public int FatigueRecovery
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.FatigueRecovery);

                return value;
            }
        }

        public int MaxActionPoint
        {
            get
            {
                var (_, value) = Stats.GetMaxValue(StatId.ActionPoint);

                return value;
            }
        }

        public int ActionPoint
        {
            get
            {
                var (_, value) = Stats.GetValue(StatId.ActionPoint);

                return value;
            }
            set => _ = Stats.SetValue(StatId.ActionPoint, value);
        }

        public override bool IsTileOccupier => true;

        public void RestoreMovement()
        {
            var (_, maxSpeed) = Stats.GetMaxValue(StatId.Speed);
            Speed = maxSpeed;
        }

        public void RestoreActionPoint()
        {
            ActionPoint = MaxActionPoint;
        }

        public void RecoverFatigue()
        {
            Fatigue += FatigueRecovery;
        }
    }
}