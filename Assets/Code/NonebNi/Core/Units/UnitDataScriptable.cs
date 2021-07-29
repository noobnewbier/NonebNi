using NonebNi.Core.BoardItems;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Units
{
    [CreateAssetMenu(fileName = nameof(UnitDataScriptable), menuName = MenuName.Data + nameof(UnitDataScriptable))]
    public class UnitDataScriptable : BoardItemDataScriptable
    {
        [SerializeField] private string unitName;
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;

        public float MaxHealth => maxHealth;
        public float Health => health;

        public UnitData ToData() => new UnitData(Icon, unitName, this);
    }
}