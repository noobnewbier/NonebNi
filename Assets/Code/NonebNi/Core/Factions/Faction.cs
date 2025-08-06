using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.DataIds;
using UnityEngine;

namespace NonebNi.Core.Factions
{
    [Serializable]
    public class Faction : IEquatable<Faction>
    {
        [SerializeField] private string id;
        [SerializeField] private bool isPlayerControlled; //TODO: feels weird do we actually need this
        [SerializeField] private List<string> allies = new();

        public Faction(string id, bool isPlayerControlled)
        {
            this.id = id;
            this.isPlayerControlled = isPlayerControlled;
        }

        public Faction(string id, bool isPlayerControlled, List<string> allies)
        {
            this.id = id;
            this.isPlayerControlled = isPlayerControlled;
            this.allies = allies;
        }


        public string Id => id;
        public bool IsPlayerControlled => isPlayerControlled;

        public List<DataId<Faction>> Allies =>
            // yes - I know it has to be serialized and this is an abomination of hell. But the world is bleak, I promise I will fix this later may god forgive me in my final days.
            allies.Select(a => new DataId<Faction>(a)).ToList();

        public bool Equals(Faction other) => Id == other.Id &&
                                             IsPlayerControlled == other.IsPlayerControlled &&
                                             Allies.Intersect(other.Allies).Count() == Allies.Count;

        public override bool Equals(object? obj) => obj is Faction other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, IsPlayerControlled, Allies);
    }
}