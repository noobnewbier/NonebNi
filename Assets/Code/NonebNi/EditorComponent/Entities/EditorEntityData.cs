using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities
{
    [Serializable]
    public abstract class EditorEntityData : ISerializationCallbackReceiver
    {
        protected EditorEntityData(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; private set; }

        public abstract EntityData ToEntityData();

        #region Serialization

        private const int GuidLength = 16;
        [SerializeField] private byte[] serializedGuid = new byte[GuidLength];

        public void OnBeforeSerialize()
        {
            if (Guid != Guid.Empty) serializedGuid = Guid.ToByteArray();
        }

        public void OnAfterDeserialize()
        {
            if (serializedGuid.Length == GuidLength) Guid = new Guid(serializedGuid);
        }

        #endregion
    }

    [Serializable]
    public class EditorEntityData<T> : EditorEntityData where T : EntityData
    {
        [SerializeField] private T entityData;

        public EditorEntityData(Guid guid, T entityData) : base(guid)
        {
            this.entityData = entityData;
        }

        public override EntityData ToEntityData() => entityData;
    }
}