using Noneb.Core.Game.Common.BoardItems;
using UnityEngine;

namespace Noneb.Core.Game.Tiles
{
    public class TileData : BoardItemData
    {
        public TileData(Sprite icon, string name, TileDataScriptable original) : base(icon, name)
        {
            Original = original;
        }

        public TileDataScriptable Original { get; }
        public float Weight => Original.Weight;
    }
}