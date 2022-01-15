using UnityEditor;

namespace NonebNi.EditorComponent.Entities.Unit
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