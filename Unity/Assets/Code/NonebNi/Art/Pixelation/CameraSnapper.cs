using UnityEngine;
using UnityEngine.Rendering;
using UnityUtils;

namespace NonebNi.Art.Pixelation
{
    //todo: can't help but feel this might not be necessary...? right?
    //todo: rotation
    //todo: zoom

    /*
     * Note:
     * - I am assuming I will have two camera, one for UI, one for anything in the 3D scene.
     * - If we have more we might have an issue, we will see.
     *
     *
     * Architect Question:
     * 1. should things snap them selves or should someone whip them to their place?
     */
    public class CameraSnapper : MonoBehaviour
    {
        private static readonly int PixelPanOffset = Shader.PropertyToID("_PixelPanOffset");
        [SerializeField] private DownSampleRenderFeature downSampleRenderFeature = null!;

        // Might need to be injected by game/level at some point.
        [SerializeField] private Camera referencedCamera = null!;

        //todo: rotation too?
        private Vector3 _cachedPosition;

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
        }

        private void BeginCameraRendering(ScriptableRenderContext ctx, Camera renderCamera)
        {
            if (referencedCamera != renderCamera) return;

            CacheTransform();
            SnapToPixelGrid();
        }

        private void CacheTransform()
        {
            _cachedPosition = referencedCamera.transform.position;
        }

        private void EndCameraRendering(ScriptableRenderContext ctx, Camera renderCamera)
        {
            if (referencedCamera != renderCamera) return;

            RestoreTransform();
        }

        private void RestoreTransform()
        {
            referencedCamera.transform.position = _cachedPosition;
        }

        private void SnapToPixelGrid()
        {
            // Pixel-Perfect logic with sub-texel panning.
            var snappedPos = CalcClosestTexelGridPos(transform.position);

            GizmosDrawer.DrawSphere(snappedPos, CalcTexelSize() / 2f, Color.blue, 0.3f);
            referencedCamera.transform.position = snappedPos;

            // Pass global UV offset to the upscale shader to compensate for movement.
            // Shader.SetGlobalVector(PixelPanOffset, new (uvOffsetX, uvOffsetY));
        }

        private Vector3 CalcClosestTexelGridPos(Vector3 position)
        {
            var texelSize = CalcTexelSize();
            //Note: if creating a bunch of them every frame is a problem we can cache this? todo: why is it different
            var trsForGridSpace = Matrix4x4.TRS(Vector3.zero, referencedCamera.transform.rotation, Vector3.one);

            // Convert to local space of the camera's visual perspective.
            var localPos = trsForGridSpace.inverse.MultiplyPoint(position);

            // Snap XY to texel grid.
            var snappedX = Mathf.Round(localPos.x / texelSize) * texelSize;
            var snappedY = Mathf.Round(localPos.y / texelSize) * texelSize;

            // Calculate rounding error.
            var snapError = new Vector2(localPos.x - snappedX, localPos.y - snappedY);

            //todo: make sub pixel panning work.
            // Convert error to UV space.
            var uvOffsetX = snapError.x / (referencedCamera.orthographicSize * 2f * referencedCamera.aspect);
            var uvOffsetY = snapError.y / (referencedCamera.orthographicSize * 2f);

            // Apply snapped position to pivot.
            var snappedPos = new Vector3(snappedX, snappedY, localPos.z);
            var texelPos = trsForGridSpace.MultiplyPoint(snappedPos);
            return texelPos;
        }

        private float CalcTexelSize()
        {
            var pixelRenderHeight = downSampleRenderFeature.RenderSize.y;
            var texelSize = referencedCamera.orthographicSize * 2f / pixelRenderHeight;
            return texelSize;
        }
    }
}