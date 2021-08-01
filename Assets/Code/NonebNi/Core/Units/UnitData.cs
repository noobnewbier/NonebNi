using NonebNi.Core.BoardItems;
using UnityEngine;

namespace NonebNi.Core.Units
{
    public class UnitData : BoardItemData
    {
        public UnitData(Sprite icon, string name, UnitDataScriptable original) : base(name)
        {
            Original = original;
        }

        public UnitDataScriptable Original { get; }

        public float MaxHealth => Original.MaxHealth;
        public float Health => Original.Health;
    }
}