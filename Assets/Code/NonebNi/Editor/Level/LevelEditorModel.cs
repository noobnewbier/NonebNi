using System;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;

namespace NonebNi.Editor.Level
{
    /// <summary>
    /// Maybe we can do some source code weaving for this ungodly boilerplate?
    /// That will probably have to be a standalone project.
    /// </summary>
    public class LevelEditorModel
    {
        private Map? _map;

        public LevelData LevelData { get; }

        public Map? Map
        {
            get => _map;
            set
            {
                _map = value;
                OnMapChanged?.Invoke(_map);
            }
        }

        public LevelEditorModel(LevelData levelData)
        {
            LevelData = levelData;
        }

        public event Action<Map?>? OnMapChanged;
    }
}