using NonebNi.Core.BoardItems;
using UnityEngine;

namespace NonebNi.Core.Constructs
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