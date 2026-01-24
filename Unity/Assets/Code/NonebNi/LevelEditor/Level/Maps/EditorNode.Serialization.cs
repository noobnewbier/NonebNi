using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Maps
{
    public partial class EditorNode : ISerializationCallbackReceiver
    {
        [SerializeField]
        private EditorEntityData<UnitData>[] serializedUnitDatas = Array.Empty<EditorEntityData<UnitData>>();

        public void OnBeforeSerialize()
        {
            serializedUnitDatas = _entityDatas.OfType<EditorEntityData<UnitData>>().ToArray();
        }

        public void OnAfterDeserialize()
        {
            _entityDatas = new List<EditorEntityData>();

            _entityDatas.AddRange(serializedUnitDatas);
        }
    }
}