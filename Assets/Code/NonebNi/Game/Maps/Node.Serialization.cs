using System;
using System.Collections.Generic;
using System.Linq;
using Code.NonebNi.Game.Entities;
using Code.NonebNi.Game.Units;
using UnityEngine;

namespace Code.NonebNi.Game.Maps
{
    public partial class Node : ISerializationCallbackReceiver
    {
        [SerializeField] private UnitData[] serializedUnitDatas = Array.Empty<UnitData>();

        public void OnBeforeSerialize()
        {
            serializedUnitDatas = _entityDatas.OfType<UnitData>().ToArray();
        }

        public void OnAfterDeserialize()
        {
            _entityDatas = new List<EntityData>();

            _entityDatas.AddRange(serializedUnitDatas);
        }
    }
}