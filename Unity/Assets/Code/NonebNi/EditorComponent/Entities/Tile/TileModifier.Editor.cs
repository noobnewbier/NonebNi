using UnityEditor;

namespace NonebNi.EditorComponent.Entities.Tile
{
    public partial class TileModifier
    {
        [CustomEditor(typeof(TileModifier))]
        [CanEditMultipleObjects]
        public class TileModifierEditor : EntityEditor { }
    }
}