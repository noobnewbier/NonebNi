using Noneb.UI.View;
using NonebNi.CustomInspector;
using UnityEditor;

namespace Noneb.UI.Editor.UIBehaviours
{
    [CustomEditor(typeof(NonebViewBehaviour))]
    public class NonebViewBehaviourInspector : UnityEditor.Editor
    {
        private NonebGUIDrawer _drawer = null!;
        private NonebViewBehaviour _self = null!;

        private void OnEnable()
        {
            _drawer = new NonebGUIDrawer(serializedObject);
            _self = (NonebViewBehaviour)target;
        }

        public override void OnInspectorGUI()
        {
            _drawer.Update();

            _drawer.DrawDefaultInspector(this);
            _drawer.DrawLabel($"IsActive: {_self.IsViewActive}");
            _drawer.DrawLabel($"InitState: {((INonebView)_self).InitState}");

            _drawer.Apply();
            Repaint();
        }

        public class EditorData : ScriptableSingleton<EditorData> { }
    }
}