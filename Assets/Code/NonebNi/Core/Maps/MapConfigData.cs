using System;
using UnityEngine;

namespace NonebNi.Core.Maps
{
    [Serializable]
    public class MapConfigData
    {
        [SerializeField] private int xSize;
        [SerializeField] private int zSize;

        public MapConfigData(int xSize, int zSize)
        {
            this.xSize = xSize;
            this.zSize = zSize;
        }

        public int GetMapWidth() => xSize;
        public int GetMapHeight() => zSize;

        public int GetTotalMapSize() => GetMapWidth() * GetMapHeight();
    }
}