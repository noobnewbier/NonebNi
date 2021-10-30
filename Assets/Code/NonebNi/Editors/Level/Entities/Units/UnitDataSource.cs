using NonebNi.Core.Units;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level.Entities.Units
{
    [CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EntityDataSource<UnitData>
    {
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;

        public float MaxHealth => maxHealth;
        public float Health => health;

        public override UnitData CreateData() => new UnitData(entityName, maxHealth, health);
    }
}