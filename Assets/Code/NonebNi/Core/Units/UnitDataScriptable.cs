using Noneb.Core.Game.Common.BoardItems;
using UnityEngine;
using UnityUtils.Constants;

namespace Noneb.Core.Game.Units
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