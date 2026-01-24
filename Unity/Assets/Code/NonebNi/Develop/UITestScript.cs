using System;
using Noneb.UI.View;
using NonebNi.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Develop
{
    //TODO: how should scene manangement with the UI state management
    public class UITestScript : MonoBehaviour
    {
        [SerializeField] private GameObject go = null!;
        [SerializeField] private GameObject[] goArray = null!;
        [SerializeField] private int primitive;
        [SerializeField] private int[] primitiveArray = null!;
        [SerializeField] private Vector3 builtInStruct;
        [SerializeField] private Vector3[] buildInStructArray = null!;


#if UNITY_EDITOR

        //TODO: warning get rid
        [Serializable]
        public class EditorData
        {
            public NonebViewBehaviour? viewToPush;
            public string subStackName = string.Empty;
        }

        [SerializeField] private EditorData editorData = new ();


        [CustomEditor(typeof(UITestScript))]
        private class InsideEditor : Editor
        {
            private NonebGUIDrawer _drawer = null!;
            private UITestScript _self = null!;

            private void OnEnable()
            {
                _drawer = new (serializedObject);
                _self = (UITestScript)target;
            }

            public override void OnInspectorGUI()
            {
                _drawer.Update();

                _drawer.DrawDefaultInspector(this);
                _drawer.DrawLabel(_drawer.FindProperty("go").boxedValue.GetType().ToString());
                _drawer.DrawLabel(_drawer.FindProperty("goArray").boxedValue.GetType().ToString());
                _drawer.DrawLabel(_drawer.FindProperty("primitive").boxedValue.GetType().ToString());
                _drawer.DrawLabel(_drawer.FindProperty("primitiveArray").boxedValue.GetType().ToString());
                _drawer.DrawLabel(_drawer.FindProperty("builtInStruct").boxedValue.GetType().ToString());
                _drawer.DrawLabel(_drawer.FindProperty("buildInStructArray").boxedValue.GetType().ToString());

                _drawer.Apply();
                Repaint();
            }
        }

#endif
    }
}