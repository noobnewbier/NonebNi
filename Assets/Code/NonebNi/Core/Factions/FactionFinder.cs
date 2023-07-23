﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NonebNi.Core.Factions
{
    public interface IFactionFinder
    {
        bool TryFindById(string id, [NotNullWhen(true)] out Faction? faction);
    }

    public class FactionFinder : IFactionFinder
    {
        private readonly Faction[] _factions;

        public FactionFinder(Faction[] factions)
        {
            _factions = factions;
        }

        public bool TryFindById(string id, [NotNullWhen(true)] out Faction? faction)
        {
            faction = _factions.FirstOrDefault(f => f.Id == id);

            return faction != null;
        }
    }
}