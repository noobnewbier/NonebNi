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

        public int GetMap2DActualWidth() => xSize;
        public int GetMap2DActualHeight() => zSize;

        public int GetMap2DArrayWidth() => GetMap2DActualWidth();

        public int GetMap2DArrayHeight() => GetMap2DActualHeight();

        public int GetTotalMapSize() => GetMap2DActualWidth() * GetMap2DActualHeight();
    }
}