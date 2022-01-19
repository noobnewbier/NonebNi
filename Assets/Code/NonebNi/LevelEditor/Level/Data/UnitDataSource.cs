using System;
using System.Linq;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.LevelEditor.Level.Data
{
    [CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField] private SkillDataSource[] skillDataSource = Array.Empty<SkillDataSource>();
        [Range(0, 100)] [SerializeField] private int initiative;


        public override EditorEntityData<UnitData> CreateData(Guid guid) =>
            new EditorEntityData<UnitData>(
                guid,
                new UnitData(
                    guid,
                    entityName,
                    maxHealth,
                    health,
                    icon,
                    skillDataSource.Select(s => s.CreateData()).ToArray(),
                    initiative
                )
            );
    }
}