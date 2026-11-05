using NonebNi.Core.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace NonebNi.Art.GrassInstancing
{
    //todo: we need to allow cases where couple accent can overlap. another type of placer...maybe in general random placer is just better than blue noise.
    //or on another thought, perhaps we should just use multiple spawner, but would that feel weird to rely on randomly overlapping as they don't have knowledge on each other.
    //todo: time for pixelation and wind?
    //todo: more about camera as well.
    [NonebUniversalInspector]
    [ExecuteAlways]
    public class GrassInstancerRunner : MonoBehaviour
    {
        private static readonly int PerInstanceData = Shader.PropertyToID("_PerInstanceData");
        [SerializeField] private MeshRenderer meshRenderer = null!;
        [SerializeField] private Material material = null!;
        [SerializeField] private Mesh toRender = null!;
        [SerializeReference, TypePicker] private GrassInstancer grassInstancer = new ();

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

        [Button(true)]
        private void Redraw()
        {
            _dirty = true;
        }

        private void OnValidate()
        {
            _dirty = true;
        }
    }
}