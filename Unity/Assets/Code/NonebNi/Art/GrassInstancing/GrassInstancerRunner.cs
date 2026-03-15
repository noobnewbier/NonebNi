using System.Diagnostics.CodeAnalysis;
using NonebNi.Core.Attributes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace NonebNi.Art.GrassInstancing
{
    [NonebUniversalInspector]
    [ExecuteAlways]
    public class GrassInstancerRunner : MonoBehaviour
    {
        private static readonly int PerInstanceData = Shader.PropertyToID("_PerInstanceData");
        [SerializeField] private MeshRenderer meshRenderer = null!;
        [SerializeField] private Material material = null!;
        [SerializeField] private Mesh toRender = null!;
        [SerializeReference, TypePicker] private GrassInstancer grassInstancer = new BlueNoiseInstancer();

        private ComputeBuffer? _buffer;
        private bool _dirty = true;
        private RenderParams _renderParams;

        private void Update()
        {
            InitRenderPrams();
            if (toRender == null) return;

            SetupDrawData();
            if (_buffer == null) return;

            Graphics.RenderMeshPrimitives(_renderParams, toRender, 0, _buffer.count);
        }

        private void OnDisable()
        {
            Discard();
        }

        private void InitRenderPrams()
        {
            if (meshRenderer == null) return;
            if (!_dirty && !transform.hasChanged) return;

            Discard();

            var boundSize = meshRenderer.bounds.size;
            _renderParams = new (material)
            {
                worldBounds = new (meshRenderer.transform.position, boundSize),
                shadowCastingMode = ShadowCastingMode.On,
                receiveShadows = true,
                matProps = new ()
            };

            transform.hasChanged = false;
            _dirty = true;
        }

        private void Discard()
        {
            _buffer?.Release();
            _buffer = null;
            _dirty = true;
        }

        private void SetupDrawData()
        {
            if (!_dirty) return;
            Discard();

            var bound = meshRenderer.bounds;
            _buffer = grassInstancer.CreatePlacementBuffer(bound);
            _renderParams.matProps.SetBuffer(PerInstanceData, _buffer);
            _dirty = false;
        }

        private void OnValidate()
        {
            _dirty = true;
        }

        private void OnDrawGizmos()
        {
            var cache = Gizmos.color;
            Gizmos.color = UnityEngine.Color.red;
            var b = meshRenderer.bounds;

            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Gizmos.DrawLine(p5, p6);
            Gizmos.DrawLine(p6, p7);
            Gizmos.DrawLine(p7, p8);
            Gizmos.DrawLine(p8, p5);

            // sides
            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p7);
            Gizmos.DrawLine(p4, p8);

            Gizmos.color = cache;
        }

        [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "We use this in our shader code.")]
        private struct InstanceData
        {
            public Matrix4x4 TRSMatrix;
        }
    }
}