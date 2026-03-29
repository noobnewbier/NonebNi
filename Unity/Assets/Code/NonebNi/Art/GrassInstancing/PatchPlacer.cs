using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Attributes;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Art.GrassInstancing
{
    public partial class GrassInstancer
    {
        [Serializable]
        protected class PatchPlacer : PointPlacer
        {
            [SerializeReference, TypePicker] private PointPlacer instancer = new BlueNoisePlacer();

            [Delayed, SerializeField] private int patchCount = 5;
            [Range(1, 100), Delayed, SerializeField] private float minPatchDistance = 5;
            [Delayed, SerializeField] private Vector2 basePatchSize = Vector2.one * 5;
            [Delayed, SerializeField] private Vector2 randomPatchSizeOffset = Vector2.one;

            public override IEnumerable<InstanceData> CreatePointsData(Bounds bounds, Vector3 center)
            {
                GizmosDrawer.DrawBound(bounds);

                var points = BlueNoise.Sampling(bounds.min, bounds.max, minPatchDistance).Take(patchCount);
                foreach (var pt in points)
                {
                    var xOffset = UnityEngine.Random.Range(-randomPatchSizeOffset.x, randomPatchSizeOffset.x);
                    var yOffset = UnityEngine.Random.Range(-randomPatchSizeOffset.y, randomPatchSizeOffset.y);
                    
                    // transforming the vec2 into a flat vec3.
                    var patchBoundSize = new Vector3(basePatchSize.x + xOffset, bounds.size.y, basePatchSize.y + yOffset);

                    var patchBounds = new Bounds(pt, patchBoundSize);
                    foreach (var data in instancer.CreatePointsData(patchBounds, center))
                    {
                        yield return data;
                    }
                }
            }
        }
    }
}