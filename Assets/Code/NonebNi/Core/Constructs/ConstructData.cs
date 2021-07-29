using Noneb.Core.Game.Common.BoardItems;
using UnityEngine;

namespace Noneb.Core.Game.Constructs
{
    public class ConstructData : BoardItemData
    {
        public ConstructData(Sprite icon, string name, ConstructDataScriptable original) : base(icon, name)
        {
            Original = original;
        }

        public ConstructDataScriptable Original { get; }
    }
}