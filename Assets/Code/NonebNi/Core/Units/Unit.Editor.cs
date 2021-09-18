using UnityEditor;

namespace NonebNi.Core.Units
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