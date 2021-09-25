using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using UnityEditor;

namespace NonebNi.Core.Tiles
{
    public partial class TileModifier : Entity<UnitData>
    {
        [CustomEditor(typeof(TileModifier))]
        [CanEditMultipleObjects]
        public class UnitEditor : EntityEditor
        {
        }
    }
}