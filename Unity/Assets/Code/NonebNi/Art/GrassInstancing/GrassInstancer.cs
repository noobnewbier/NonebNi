using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NonebNi.Core.Attributes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Art.GrassInstancing
{
    [Serializable]
    public partial class GrassInstancer
    {
        [SerializeReference, TypePicker] private PointPlacer placer = new BlueNoisePlacer();

        public ComputeBuffer CreatePlacementBuffer(Bounds bounds)
        {
            GizmosDrawer.Dismiss(this);
            using (GizmosDrawer.Category("GrassInstancing"))
            using (GizmosDrawer.ForeverTillDismiss(this))
            {
                var points = placer.CreatePointsData(bounds, bounds.center).ToArray();
                var size = UnsafeUtility.SizeOf<PointPlacer.InstanceData>();

                var buffer = new ComputeBuffer(points.Length, size);
                buffer.SetData(points);
                return buffer;
            }
        }

        protected abstract class PointPlacer
        {
            // sucks that it has to be public - don't really think we have other choice though.
            public abstract IEnumerable<InstanceData> CreatePointsData(Bounds bounds, Vector3 center);

            [SuppressMessage("ReSharper", "NotAccessedField.Global", Justification = "We use this in our shader code.")]
            public struct InstanceData
            {
                public Matrix4x4 TRSMatrix;
            }
        }
    }
}