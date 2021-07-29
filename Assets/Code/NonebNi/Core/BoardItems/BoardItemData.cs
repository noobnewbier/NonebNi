using UnityEngine;

namespace Noneb.Core.Game.Common.BoardItems
{
    /// <summary>
    /// it's slightly hacky to prevent the need to create a bunch of scriptable of each new levels,
    /// another approach is to allow scriptable to reference another scriptable to make a prefab like structure,
    /// but that would be way too complicated for our needs for now
    /// </summary>
    public abstract class BoardItemData
    {
        protected BoardItemData(Sprite icon, string name)
        {
            Icon = icon;
            Name = name;
        }

        public Sprite Icon { get; }
        public string Name { get; }
    }
}