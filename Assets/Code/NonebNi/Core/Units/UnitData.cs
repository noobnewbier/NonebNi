using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Units
{
    public class UnitData : EntityData
    {
        public UnitDataSource Original { get; }

        public float MaxHealth => Original.MaxHealth;
        public float Health => Original.Health;

        public UnitData(Sprite icon, string name) : base(name)
        {
        }
    }
}