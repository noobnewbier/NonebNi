using System.Collections.Generic;

namespace NonebNi.Core.Entities
{
    /// <summary>
    /// Hard coded factions - no need for anything fancier than this for now - when the time comes this will be a json but this is not the time
    /// </summary>
    public static class FactionsData
    {
        public static readonly Faction Player = new(nameof(Player), nameof(AllyNpc));
        public static readonly Faction AllyNpc = new(nameof(AllyNpc), nameof(Player));
        public static readonly Faction EnemyNpc = new(nameof(EnemyNpc));
        public static readonly Faction Neutral = new(nameof(Neutral));

        public static readonly IEnumerable<Faction> AllFactions = new[] { Player, AllyNpc, EnemyNpc, Neutral };
    }
}