using System;
using System.Collections.Generic;
using System.Linq;
using Code.NonebNi.EditorComponent.Entities;
using NonebNi.Game.Units;
using UnityEngine;

namespace NonebNi.Editors.Level.Data
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