using UnityEngine;
using UnityUtils;

namespace Code.NonebNi.Game.Level
{
    /// <summary>
    /// The backing data for a <see cref="Game" />
    /// </summary>
    public class LevelDataSource : ScriptableObject
    {
        [SerializeField] private LevelData data = null!;

        public LevelData GetData() => data.DeepCopy();

        public void WriteData(LevelData levelData)
        {
            //with the goal of making the Game lib absolutely editor agnostics, the data dirtying needs to be done by the caller instead   
            data = levelData;
        }
    }
}