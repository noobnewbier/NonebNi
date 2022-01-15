using NonebNi.Ui;
using UnityEditor;
using UnityEngine;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomEditor(typeof(PrototypeDieAnimationControl))]
    public class PrototypeDieAnimationControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (serializedObject.targetObjects.Length > 1) return;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                var typedTarget = (PrototypeDieAnimationControl)target;
                if (GUILayout.Button(nameof(PrototypeDieAnimationControl.Play))) typedTarget.Play(new Context());
            }
        }
    }
}