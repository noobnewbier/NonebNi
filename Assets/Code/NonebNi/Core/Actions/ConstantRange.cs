using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class ConstantRange : Range
    {
        [SerializeField] private int range;

        public ConstantRange(int range)
        {
            this.range = range;
        }

        public override int CalculateRange(EntityData _) => range;
    }
}