using NonebNi.Core.Sequences;
using NonebNi.Ui.Animation;
using NonebNi.Ui.Animation.Sequence;
using UnityEditor;
using UnityEngine;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomEditor(typeof(PrototypeAnimationControl))]
    public class PrototypeAnimationControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (serializedObject.targetObjects.Length > 1) return;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                var typedTarget = (PrototypeAnimationControl)target;
                if (GUILayout.Button(nameof(DieSequence))) typedTarget.Play(new DieAnimSequence());
                if (GUILayout.Button(nameof(DamageSequence))) typedTarget.Play(new ReceivedDamageAnimSequence());
            }
        }
    }
}