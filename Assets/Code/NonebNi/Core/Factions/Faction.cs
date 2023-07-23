using System;
using UnityEngine;

namespace NonebNi.Core.Factions
{
    [Serializable]
    public class Faction
    {
        public Faction(string id, params string[] alliesId)
        {
            Id = id;
            AlliesId = alliesId;
        }

        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string[] AlliesId { get; private set; }
    }
}