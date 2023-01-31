using System;
using UnityEngine;
using UnityUtils.SerializableGuid;

namespace NonebNi.Core.Entities
{
    /// <summary>
    /// Todo: add in SAUCE so the change of one data reflects on all level(or just all units in the same scene)
    /// </summary>
    [Serializable]
    public abstract class EntityData
    {
        [SerializeField] private string name;
        [SerializeField] private SerializableGuid serializableGuid;
        [field: SerializeField] public string FactionId { get; private set; }
        
        
        protected EntityData(string name, Guid serializableGuid, string factionId)
        {
            this.name = name;
            this.serializableGuid = new SerializableGuid(serializableGuid);
            FactionId = factionId;
        }

        public SerializableGuid Guid => serializableGuid;
        public string Name => name;
    }
}