using UnityEditor;

namespace NonebNi.Editors.Level.Entities.Tile
{
    public partial class TileModifier : Entity<TileEntityData>
    {
        [CustomEditor(typeof(TileModifier))]
        [CanEditMultipleObjects]
        public class UnitEditor : EntityEditor
        {
        }
    }
}