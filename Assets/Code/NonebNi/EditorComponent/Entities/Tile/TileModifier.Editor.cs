using UnityEditor;

namespace Code.NonebNi.EditorComponent.Entities.Tile
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