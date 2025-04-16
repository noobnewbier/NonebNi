using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.EditorComponent.Entities.Unit
{
    [Serializable, CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public partial class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField] private string[] actionIds = Array.Empty<string>();
        [Range(0, 100), SerializeField] private int initiative;
        [SerializeField] private int speed;
        [SerializeField] private int focus;
        [SerializeField] private int strength;
        [SerializeField] private int armor;
        [SerializeField] private int weaponRange;
        [SerializeField] private int fatigue;
        [SerializeField] private int maxFatigue;


        public override EditorEntityData<UnitData> CreateData(Guid guid, string factionId)
        {
            var actions = new List<NonebAction>();
            foreach (var a in actionIds.Select(ActionDatas.Find))
            {
                if (a == null)
                {
                    Log.Error($"Failed to find action with id: {a}");
                    continue;
                }

                actions.Add(a);
            }

            return new EditorEntityData<UnitData>(
                guid,
                new UnitData(
                    guid,
                    actions,
                    icon,
                    entityName,
                    factionId,
                    maxHealth,
                    health,
                    initiative,
                    speed,
                    focus,
                    strength,
                    armor,
                    weaponRange,
                    fatigue,
                    maxFatigue
                )
            );
        }
    }
}