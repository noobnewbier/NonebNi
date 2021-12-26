using System;
using UnityEngine;

namespace NonebNi.Game.Entities
{
    /// <summary>
    /// Todo: add in SAUCE so the change of one data reflects on all level(or just all units in the same scene)
    /// </summary>
    [Serializable]
    public abstract class EntityData
    {
        [SerializeField] private string name;

        protected EntityData(string name)
        {
            this.name = name;
        }
    }
}