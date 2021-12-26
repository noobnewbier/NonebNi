using System;
using Code.NonebNi.Game.Entities;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    [Serializable]
    public abstract class EditorEntityData
    {
        protected EditorEntityData(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; private set; }

        public abstract EntityData ToEntityData();

        #region Serialization

        [SerializeField] private byte[] serializedGuid = new byte[16]; //16 is the guid's length in bytes

        public void OnBeforeSerialize()
        {
            if (Guid != Guid.Empty) serializedGuid = Guid.ToByteArray();
        }

        public void OnAfterDeserialize()
        {
            if (serializedGuid.Length == 16) Guid = new Guid(serializedGuid);
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

        public T EntityData => entityData;
        public override EntityData ToEntityData() => entityData;
    }
}