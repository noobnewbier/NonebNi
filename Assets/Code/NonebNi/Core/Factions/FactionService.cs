using System.Collections.Generic;
using NonebNi.Core.DataIds;
using Unity.Logging;

namespace NonebNi.Core.Factions
{
    public interface IFactionService
    {
        bool IsAlly(DataId<Faction> a, DataId<Faction> b);
        Faction FindFaction(DataId<Faction> factionId);
    }

    public class FactionService : IFactionService
    {
        private readonly Dictionary<DataId<Faction>, Faction> _factions;

        public FactionService(Faction[] factions)
        {
            _factions = new Dictionary<DataId<Faction>, Faction>();
            foreach (var faction in factions) _factions[faction.Id] = faction;
        }

        public bool IsAlly(DataId<Faction> a, DataId<Faction> b)
        {
            if (a == b) return true;

            var factionA = GetOrCreateFaction(a);
            var factionB = GetOrCreateFaction(b);

            return factionA.Allies.Contains(b) && factionB.Allies.Contains(a);
        }

        public Faction FindFaction(DataId<Faction> factionId) => GetOrCreateFaction(factionId);

        private Faction GetOrCreateFaction(DataId<Faction> factionId)
        {
            if (!_factions.TryGetValue(factionId, out var faction))
            {
                Log.Warning($"You are trying to look for {factionId} which doesn't exist, this seems weird but we are rolling with it by creating a dummy faction");
                _factions[factionId] = faction = new Faction(factionId, false);
            }

            return faction;
        }
    }
}