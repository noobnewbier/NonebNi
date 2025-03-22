using System;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities.Unit
{
    public partial class UnitDataSource
    {
        [SerializeField] private EditorData data = new();

        [Serializable]
        public class EditorData { }

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
    }
}