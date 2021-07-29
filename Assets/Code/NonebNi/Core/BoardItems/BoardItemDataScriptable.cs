using UnityEngine;

namespace Noneb.Core.Game.Common.BoardItems
{
    public abstract class BoardItemDataScriptable : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        protected Sprite Icon => icon;
    }
}