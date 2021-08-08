using NonebNi.Core.Maps;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Level
{
    /// <summary>
    /// Created this as we anticipate the need for custom BGMs and stuffs in the future
    /// </summary>
    [CreateAssetMenu(fileName = nameof(LevelDataScriptable), menuName = MenuName.Data + nameof(LevelDataScriptable))]
    public class LevelDataScriptable : ScriptableObject
    {
        public MapConfig mapConfig;
        public WorldConfig worldConfig;
    }
}