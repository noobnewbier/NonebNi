using System;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Game.Maps
{
    [CreateAssetMenu(fileName = nameof(MapConfigScriptable), menuName = MenuName.Data + nameof(MapConfigScriptable))]
    public class MapConfigScriptable : ScriptableObject
    {
        private static readonly Lazy<MapConfigScriptable> LazyEmpty = new Lazy<MapConfigScriptable>(() => Create(0, 0));

        [Range(1, 100)] [SerializeField] private int xSize;
        [Range(1, 100)] [SerializeField] private int zSize;
        public static MapConfigScriptable Empty => LazyEmpty.Value;

        public int GetMap2DActualWidth() => xSize;
        public int GetMap2DActualHeight() => zSize;

        public static MapConfigScriptable Create(int xSize, int zSize)
        {
            var instance = CreateInstance<MapConfigScriptable>();

            instance.xSize = xSize;
            instance.zSize = zSize;

            return instance;
        }

        public MapConfigData CreateData() => new MapConfigData(xSize, zSize);

        public int GetMap2DArrayWidth()
        {
            var zeroBasedHeight = Mathf.Max(GetMap2DActualHeight() - 1, 0);
            return GetMap2DActualWidth() + zeroBasedHeight / 2 + zeroBasedHeight % 2;
        }

        public int GetMap2DArrayHeight() => GetMap2DActualHeight();

        public int GetTotalMapSize() => GetMap2DActualWidth() * GetMap2DActualHeight();
    }
}