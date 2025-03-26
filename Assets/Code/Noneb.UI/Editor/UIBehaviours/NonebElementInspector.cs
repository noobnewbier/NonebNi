using Noneb.UI.Element;
using NonebNi.CustomInspector;
using UnityEditor;

namespace Noneb.UI.Editor.UIBehaviours
{
    [CustomEditor(typeof(NonebElement))]
    public class NonebElementInspector : UnityEditor.Editor
    {
        private NonebGUIDrawer _drawer = null!;
        private NonebElement _self = null!;

        private void OnEnable()
        {
            _drawer = new NonebGUIDrawer(serializedObject);
            _self = (NonebElement)target;
        }

        public override void OnInspectorGUI()
        {
            _drawer.Update();

            _drawer.DrawDefaultInspector(this);
            _drawer.DrawLabel($"IsActive: {_self.IsElementActive}");

            _drawer.Apply();
            Repaint();
        }

        public class EditorData : ScriptableSingleton<EditorData> { }
    }
}