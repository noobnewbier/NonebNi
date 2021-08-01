using UnityEngine;

namespace NonebNi.Core.BoardItems
{
    public abstract class BoardItemDataScriptable : ScriptableObject
    {
        [SerializeField] private Sprite icon = null!;
        protected Sprite Icon => icon;
    }
}