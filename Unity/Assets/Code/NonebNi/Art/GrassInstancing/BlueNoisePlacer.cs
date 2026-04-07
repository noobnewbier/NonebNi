using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils;
using Random = UnityEngine.Random;

namespace NonebNi.Art.GrassInstancing
{
    public partial class GrassInstancer
    {
        [Serializable]
        protected class BlueNoisePlacer : PointPlacer
        {
            [Delayed] [Min(0.01f)] [SerializeField] private float minDistBetweenInstance = 0.5f;
            [Delayed] [SerializeField] private Vector3 minScale = Vector3.one;
            [Delayed] [SerializeField] private Vector3 maxScale = Vector3.one;
            [Delayed] [SerializeField] private float yOffsetRatio = 0.5f;
            [Delayed] [Range(0f, 360f)] [SerializeField] private float rotationRange;
            [Delayed, SerializeField] private Vector3 patchScaleFactor = Vector3.zero;
            [SerializeField] private SDFType sdfType;


            public override IEnumerable<InstanceData> CreatePointsData(Bounds bounds, Vector3 center)
            {
                if (minDistBetweenInstance <= 0) minDistBetweenInstance = 0.5f;

                GizmosDrawer.DrawBound(bounds);

                var radius = bounds.size.x / 2f;
                Func<Vector3, bool>? sdfFunc = sdfType switch
                {
                    SDFType.None => null,
                    SDFType.Circle => p =>
                    {
                        var scale = patchScaleFactor + Vector3.one;
                        var scaledPoint = Vector3.Scale(p, scale);
                        var scaledCenter = Vector3.Scale(bounds.center, scale);
                        var dist = Vector3.Distance(scaledPoint, scaledCenter);
                        return dist <= radius;
                    },
                    _ => throw new ArgumentOutOfRangeException()
                };

                Func<Vector3>? startPointFunc = sdfType switch
                {
                    SDFType.None => null,
                    SDFType.Circle => () =>
                    {
                        var rnd = Random.insideUnitCircle;
                        var point = new Vector3(rnd.x, 0, rnd.y) + bounds.center;
                        return point;
                    },
                    _ => throw new ArgumentOutOfRangeException()
                };

                var points = BlueNoise.Sampling(bounds.min, bounds.max, minDistBetweenInstance, startPointFunc, sdfFunc).ToArray();
                var datas = new InstanceData[points.Length];

                for (var i = 0; i < datas.Length; i++)
                {
                    var pt = points[i];
                    GizmosDrawer.DrawSphere(pt);
                    var range = rotationRange / 2f;
                    var rotInDegree = Random.Range(-range, range);
                    var rotInRadians = rotInDegree * Mathf.Deg2Rad;

                    // Note: position is relative
                    var scale = new Vector3
                    (
                        Random.Range(minScale.x, maxScale.x),
                        Random.Range(minScale.y, maxScale.y),
                        Random.Range(minScale.z, maxScale.z)
                    );
                    var translation = new Vector3(pt.x, pt.y + yOffsetRatio * scale.y, pt.z) - center;
                    datas[i] = new ()
                    {
                        TRSMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scale),
                        RotateInCameraAxis = rotInRadians
                    };
                }

                return datas;
            }

            public enum SDFType
            {
                None,
                Circle
            }
        }
    }
}