using UnityEditor;

namespace NonebNi.EditorComponent.Entities.Tile
{
    public partial class TileModifier : EditorEntity<EditorEntityData<TileEntityData>>
    {
        [CustomEditor(typeof(TileModifier))]
        [CanEditMultipleObjects]
        public class UnitEditor : EntityEditor
        {
        }
    }
}