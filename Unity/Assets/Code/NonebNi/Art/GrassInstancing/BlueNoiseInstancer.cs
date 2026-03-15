using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NonebNi.Art.GrassInstancing
{
    [Serializable]
    public class BlueNoiseInstancer : GrassInstancer
    {
        [Delayed] [Min(0.01f)] [SerializeField] private float minDistBetweenInstance = 0.5f;
        [Delayed] [SerializeField] private Vector3 minScale = Vector3.one;
        [Delayed] [SerializeField] private Vector3 maxScale = Vector3.one;
        [Delayed] [SerializeField] private float yOffset = 0.5f;
        [Delayed] [Range(0f, 360f)] [SerializeField] private float rotationRange;

        public override ComputeBuffer CreatePlacementBuffer(Bounds bounds)
        {
            if (minDistBetweenInstance <= 0) minDistBetweenInstance = 0.5f;

            var points = BlueNoise.Sampling(bounds.min, bounds.max, minDistBetweenInstance).ToArray();
            var datas = new InstanceData[points.Length];

            for (var i = 0; i < datas.Length; i++)
            {
                var pt = points[i];
                var rotInDegree = Random.Range(0, rotationRange);

                // Note: position is relative
                var translation = new Vector3(pt.x, pt.y + yOffset, pt.z) - bounds.center;
                var rot = Quaternion.AngleAxis(rotInDegree, Vector3.up);
                var scale = new Vector3
                (
                    Random.Range(minScale.x, maxScale.x),
                    Random.Range(minScale.y, maxScale.y),
                    Random.Range(minScale.z, maxScale.z)
                );
                datas[i] = new ()
                {
                    TRSMatrix = Matrix4x4.TRS(translation, rot, scale)
                };
            }

            var size = UnsafeUtility.SizeOf<InstanceData>();
            var buffer = new ComputeBuffer(points.Length, size);
            buffer.SetData(datas);

            return buffer;
        }
    }
}