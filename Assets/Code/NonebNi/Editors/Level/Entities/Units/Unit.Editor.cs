using UnityEditor;

namespace NonebNi.Editors.Level.Entities.Units
{
    public partial class Unit
    {
        [CustomEditor(typeof(Unit))]
        [CanEditMultipleObjects]
        public class UnitEditor : EntityEditor
        {
        }
    }
}