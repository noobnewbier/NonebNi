using System;
using Noneb.Localization.Runtime;
using NonebNi.Core.Actions;
using NonebNi.Core.Tiles;
using UnityEngine;
using UnityUtils.Serialization;

namespace NonebNi.Core.Entities
{
    /// <summary>
    /// 1. An Entity Data in level MUST exist on at least one node
    /// (if it doesn't it just doesn't exist on the level, which is fine for unit testing etc)
    /// 
    /// 2. An Entity Data CAN exist on more than one node, this is useful for <see cref="TileModifierData" />
    /// where a single obstacle might span over multiple tiles.
    ///
    /// Todo:
    /// Rename this to Entity? Not sure about the data suffix here.
    /// add in SAUCE so the change of one data reflects on all level(or just all units in the same scene)
    /// OR we can just resync every scene file to update it with the newest data -> either way works? But I suppose one with
    /// SAUCE makes more sense(as that way we don't need reload for a better workflow?
    /// </summary>
    [Serializable]
    public abstract class EntityData : IActionTarget
    {
        [SerializeField] private NonebLocString name;
        [SerializeField] private SerializableGuid serializableGuid;

        [field: SerializeField] public string FactionId { get; private set; }

        protected EntityData(NonebLocString name, Guid serializableGuid, string factionId)
        {
            this.name = name;
            this.serializableGuid = new SerializableGuid(serializableGuid);
            FactionId = factionId;
        }

        public abstract bool IsTileOccupier { get; }

        public SerializableGuid Guid => serializableGuid;
        public NonebLocString Name => name;

        public bool IsSystem => this == SystemEntity.Instance;

        public override string ToString() => Name.GetLocalized();
    }
}