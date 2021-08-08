using NonebNi.Core.Maps;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Constants;

namespace NonebNi.Core.Level
{
    /// <summary>
    /// Created this as we anticipate the need for custom BGMs and stuffs in the future
    /// </summary>
    [CreateAssetMenu(fileName = nameof(LevelDataScriptable), menuName = MenuName.Data + nameof(LevelDataScriptable))]
    public class LevelDataScriptable : ScriptableObject
    {
        [FormerlySerializedAs("mapConfig")] public MapConfigScriptable mapConfigScriptable;
        public WorldConfig worldConfig;
    }
}