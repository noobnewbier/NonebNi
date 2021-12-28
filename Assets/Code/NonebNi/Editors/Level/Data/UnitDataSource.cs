﻿using System;
using System.Linq;
using Code.NonebNi.EditorComponent.Entities;
using NonebNi.Core.Units;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level.Data
{
    [CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField] private Sprite icon;
        [SerializeField] private SkillDataSource[] skillDataSource = Array.Empty<SkillDataSource>();


        public int MaxHealth => maxHealth;
        public int Health => health;

        public override EditorEntityData<UnitData> CreateData(Guid guid) =>
            new EditorEntityData<UnitData>(
                guid,
                new UnitData(
                    entityName,
                    maxHealth,
                    health,
                    icon,
                    skillDataSource.Select(s => s.CreateData()).ToArray()
                )
            );
    }
}