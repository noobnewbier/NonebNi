using System;
using System.Collections.Generic;
using NonebNi.Core.Factions;

namespace NonebNi.EditorComponent
{
    /// <summary>
    ///     Fed in externally, mainly act as a bridge between EditorComponent and LevelEditor to avoid dependency hell.
    ///     If only editor assembly can handle mono behaviour, dear Unity.
    /// </summary>
    public static class EditorComponentSceneData
    {
        public static IReadOnlyList<Faction> AvailableFactions { get; private set; } = Array.Empty<Faction>();

        public static void Provide(Faction[] factionsInLevel)
        {
            AvailableFactions = factionsInLevel;
        }

        public static void Clear()
        {
            AvailableFactions = Array.Empty<Faction>();
        }
    }
}