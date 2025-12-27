using System;
using UnityEngine;

namespace NonebNi.Core.DataIds
{
    //todo: a way to read/write data
    /// <summary>
    /// Idea of this is to find data without the reference, so you can serialize reference to data at relative ease.
    /// This would only work if we could retrieve the data easily though.
    /// Really don't dig doing it but a big old static class might not be too bad - do we really need reference counting to
    /// release the resources?
    /// Note:
    /// The name is a bit bonk
    /// </summary>
    [Serializable]
    public record DataId<T>
    {
        [field: SerializeField] public string Id { get; private set; } = string.Empty;

        public DataId(string id)
        {
            Id = id;
        }

        public bool IsValid() => !string.IsNullOrEmpty(Id);

        public static implicit operator string(DataId<T> dataId) => dataId.Id;

        public static implicit operator DataId<T>(string id) => new(id);
    }
}