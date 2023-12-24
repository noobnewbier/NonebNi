﻿using System;
using System.Linq;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities.Skills;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.EditorComponent.Entities.Unit
{
    [CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField] private SkillDataSource[] skillDataSource = Array.Empty<SkillDataSource>();
        [Range(0, 100)] [SerializeField] private int initiative;
        [SerializeField] private int speed;
        [SerializeField] private int focus;
        [SerializeField] private int strength;
        [SerializeField] private int armor;
        [SerializeField] private int weaponRange;

        public override EditorEntityData<UnitData> CreateData(Guid guid, string factionId) =>
            new(
                guid,
                new UnitData(
                    guid,
                    entityName,
                    factionId,
                    maxHealth,
                    health,
                    icon,
                    skillDataSource.Select(s => s.CreateData()).ToArray(),
                    initiative,
                    speed,
                    focus,
                    strength,
                    armor,
                    weaponRange
                )
            );
    }
}