using System;

namespace NonebNi.Core.Entities
{
    public class SystemEntity : EntityData
    {
        public static readonly SystemEntity Instance = new("system", System.Guid.Empty, "system");

        private SystemEntity(string name, Guid serializableGuid, string factionId) : base(
            name,
            serializableGuid,
            factionId
        ) { }

        public override bool IsTileOccupier => false;
    }
}