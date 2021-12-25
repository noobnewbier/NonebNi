using System;
using Code.NonebNi.EditorComponent.Entities;
using Code.NonebNi.Game.Units;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level.Data
{
    [CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
    {
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;

        public float MaxHealth => maxHealth;
        public float Health => health;

        public override EditorEntityData<UnitData> CreateData(Guid guid) =>
            new EditorEntityData<UnitData>(guid, new UnitData(entityName, maxHealth, health));
    }
}