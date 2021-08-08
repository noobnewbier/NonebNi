using UnityEngine;

namespace NonebNi.Core.Maps
{
    public class MapConfigData
    {
        private readonly int _xSize;
        private readonly int _zSize;

        public MapConfigData(int xSize, int zSize)
        {
            _xSize = xSize;
            _zSize = zSize;
        }

        public int GetMap2DActualWidth() => _xSize;
        public int GetMap2DActualHeight() => _zSize;

        public int GetMap2DArrayWidth()
        {
            var zeroBasedHeight = Mathf.Max(GetMap2DActualHeight() - 1, 0);
            return GetMap2DActualWidth() + zeroBasedHeight / 2 + zeroBasedHeight % 2;
        }

        public int GetMap2DArrayHeight() => GetMap2DActualHeight();

        public int GetTotalMapSize() => GetMap2DActualWidth() * GetMap2DActualHeight();
    }
}