using System;
using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Factions
{
    [Serializable]
    public struct Faction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public bool IsPlayerControlled { get; private set; }
        [field: SerializeField] public string[] Allies { get; private set; }

        public Faction(string id, bool isPlayerControlled, string[] allies)
        {
            Id = id;
            IsPlayerControlled = isPlayerControlled;
            Allies = allies;
        }

        public bool Equals(Faction other) => Id == other.Id &&
                                             IsPlayerControlled == other.IsPlayerControlled &&
                                             Allies.Intersect(other.Allies).Count() == Allies.Length;

        public override bool Equals(object? obj) => obj is Faction other && Equals(other);


        public override int GetHashCode() => HashCode.Combine(Id, IsPlayerControlled, Allies);
    }
}