using System;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Maps
{
    [CreateAssetMenu(fileName = nameof(MapConfig), menuName = MenuName.Data + nameof(MapConfig))]
    public class MapConfig : ScriptableObject
    {
        private static readonly Lazy<MapConfig> LazyEmpty = new Lazy<MapConfig>(() => Create(0, 0));

        [Range(1, 100)] [SerializeField] private int xSize;
        [Range(1, 100)] [SerializeField] private int zSize;
        public static MapConfig Empty => LazyEmpty.Value;

        public int GetMap2DActualWidth() => xSize;
        public int GetMap2DActualHeight() => zSize;

        public static MapConfig Create(int xSize, int zSize)
        {
            var instance = CreateInstance<MapConfig>();

            instance.xSize = xSize;
            instance.zSize = zSize;

            return instance;
        }

        #region Methods - Consider moving them to a service or repo...

        public int GetMap2DArrayWidth()
        {
            var zeroBasedHeight = Mathf.Max(GetMap2DActualHeight() - 1, 0);
            return GetMap2DActualWidth() + zeroBasedHeight / 2 + zeroBasedHeight % 2;
        }

        public int GetMap2DArrayHeight() => GetMap2DActualHeight();

        public int GetTotalMapSize() => GetMap2DActualWidth() * GetMap2DActualHeight();

        #endregion
    }
}