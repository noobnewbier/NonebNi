using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Units;
using NonebNi.CustomInspector;
using Unity.Logging;
using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.EditorComponent.Entities.Unit
{
    [Serializable, CreateAssetMenu(fileName = nameof(UnitDataSource), menuName = MenuName.Data + nameof(UnitDataSource))]
    public class UnitDataSource : EditorEntityDataSource<EditorEntityData<UnitData>>
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

#if UNITY_EDITOR

        [Serializable]
        public class EditorData { }

        [SerializeField] private EditorData data = new();

        [CustomEditor(typeof(UnitDataSource))]
        private class InsideEditor : Editor
        {
            private NonebGUIDrawer _drawer = null!;
            private UnitDataSource _self = null!;

            private void OnEnable()
            {
                _drawer = new NonebGUIDrawer(serializedObject);
                _self = (UnitDataSource)target;
            }

            public override void OnInspectorGUI()
            {
                _drawer.Update();

                _drawer.DrawProperty(nameof(health));
                _drawer.DrawProperty(nameof(maxHealth));
                _drawer.DrawAutoCompleteProperty(
                    nameof(actionIds),
                    () => ActionDatas.Actions.Select(a => a.Id).ToArray()
                );
                _drawer.DrawProperty(nameof(initiative));
                _drawer.DrawProperty(nameof(speed));
                _drawer.DrawProperty(nameof(focus));
                _drawer.DrawProperty(nameof(strength));
                _drawer.DrawProperty(nameof(armor));
                _drawer.DrawProperty(nameof(weaponRange));
                _drawer.DrawProperty(nameof(fatigue));
                _drawer.DrawProperty(nameof(maxFatigue));

                _drawer.Apply();
            }
        }

#endif
    }
}