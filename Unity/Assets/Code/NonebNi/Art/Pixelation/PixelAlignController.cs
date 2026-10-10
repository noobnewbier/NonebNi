using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtils;

namespace NonebNi.Art.Pixelation
{
    /*
     * TODO OUTLINE:
     * few options
     *
     * ### Outline in screenspace, color offset by pixels
     * - Feels too "restricted" -> want color control on outline on individual obj.
     * - Might be useful for dramatic effect but not now...?
     *
     * ### Outline in Shader, in high res first.
     * - will need reference on downscaled color/depth/normals first...?
     */
    
    
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
    /// <summary>
    /// This does three things:
    /// 1. Making sure your camera is aligned with the pixel grid when rendering
    /// 2. Shifting the render texture(which we use to render our scene) according to the snap distance to ensure a smooth camera (perceived) movement
    /// 3. Making sure the quad that we uses to show our RT is correctly sized.
    /// Note - Why no RawImage
    /// We found out the hard way that RawImage(or canvas's component in general) are all rendered one frame later than we need.
    /// And as we need frame perfect movement in this script, trying to use RawImage wouldn't work, and hence we are using a quad instead.
    /// There might be something we can do by jiggling with the event order, but good god there's 0 documentation around this, and I am tired.
    /// Future me that's a you problem - potential relevant thread: https://discussions.unity.com/t/canvas-renders-one-frame-behind/671272
    /// </summary>
    //todo: run in edit mode? todo: figure out render pipeline(start resolution and end resolution.
    public class PixelAlignController : MonoBehaviour
    {
        private static readonly int OverRenderSize = Shader.PropertyToID("_OverRenderSize");
        private static readonly int PixelToMove = Shader.PropertyToID("_PixelToMove");
        private static readonly int TargetResolution = Shader.PropertyToID("_TargetResolution");
        // Might need to be injected by game/level at some point.
        [SerializeField] private PixelRendererFeature pixelRenderFeature = null!;
        [SerializeField] private Camera sceneCamera = null!;
        [SerializeField] private MeshRenderer rtQuad = null!;
        [SerializeField] private Camera cameraForRenderTexture = null!;
        [SerializeField] private bool isCamSmoothing;

        private Vector3 _cachedCameraPosition;

        public Action SnapRequested;
        public Action UnsnapRequested;

        private void Update()
        {
            using (GizmosDrawer.Category("pixel-align"))
            {
                const int drawDimension = 4;
                var curr = FindClosestTexelPos(sceneCamera.transform.position);
                var center = sceneCamera.transform.position;
                var texelSize = GetTexelSize();
                var gridReferenceCenter = new Vector3
                (
                    Mathf.RoundToInt(center.x / (texelSize * drawDimension * 2)) * (texelSize * drawDimension * 2),
                    center.y,
                    Mathf.RoundToInt(center.z / (texelSize * drawDimension * 2)) * (texelSize * drawDimension * 2)
                );

                // Edge cases when on edge this is not drawn properly - I can't be asked to debug this but this will do.
                GizmosDrawer.DrawLine(curr.texelPos, center, Color.green);
                GizmosDrawer.DrawSphere(curr.texelPos, texelSize / 8f, Color.yellow);

                GizmosDrawer.DrawSphere(sceneCamera.transform.position, texelSize / 8f, Color.green);
                for (var z = -drawDimension; z < drawDimension; z++)
                for (var y = 0; y < 1; y++)
                for (var x = -drawDimension; x < drawDimension; x++)
                {
                    var xOffset = x * texelSize;
                    var yOffset = y * texelSize;
                    var zOffset = z * texelSize;
                    var dist = new Vector3(xOffset, yOffset, zOffset);
                    var pos = gridReferenceCenter + dist;

                    var (texelPos, snapErr) = FindClosestTexelPos(pos);
                    if (texelPos == curr.texelPos) { }
                    else
                    {
                        GizmosDrawer.DrawSphere(texelPos, texelSize / 8f, Color.magenta);
                    }
                }
            }
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
            RenderPipelineManager.activeRenderPipelineCreated += OnPipelineCreated;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
            RenderPipelineManager.activeRenderPipelineCreated -= OnPipelineCreated;
        }

        private void OnPipelineCreated()
        {
            //todo: clean this up before I go super sayan.
            
            // We are rendering the screen by one texel larger - the upscaling shader need to know this ratio so it can actually do the sampling correctly.
            var overRenderSize = pixelRenderFeature.GetRenderSizeForSmoothPixelCamera();
            var targetRes = pixelRenderFeature.GetTargetResolution();
            var baseRes = pixelRenderFeature.GetBaseResolution();

            //Adapt quad size to screen size - actually need to do it as we resize the screen as well if even just for debugging?
            var rtCameraTexture = new RenderTexture(overRenderSize.x, overRenderSize.y, 32, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point //todo: update point mode.
            };
            sceneCamera.targetTexture = rtCameraTexture;
            SetTexture(rtCameraTexture);

            {
                //todo: wat - change this noneb

                var ratioFromBaseToTargetRes = (double)targetRes.y / baseRes.y;
                var pixelPerOrthoSizeForRTCam = (double)cameraForRenderTexture.orthographicSize / targetRes.y;
                var sceneCamRequiredOrthoSizeForOverRender = pixelPerOrthoSizeForRTCam * overRenderSize.y;
                sceneCamRequiredOrthoSizeForOverRender *= ratioFromBaseToTargetRes;
                sceneCamera.orthographicSize = (float)sceneCamRequiredOrthoSizeForOverRender;
            }

            Shader.SetGlobalVector(OverRenderSize, new Vector4(overRenderSize.x, overRenderSize.y, 0, 0));
            Shader.SetGlobalVector(TargetResolution, new Vector4(targetRes.x, targetRes.y, 0, 0));
        }

        private void BeginCameraRendering(ScriptableRenderContext ctx, Camera renderCamera)
        {
            if (renderCamera != sceneCamera) return;

            CacheTransformBeforeRendering();

            var (texelPos, snapErr) = FindClosestTexelPos(sceneCamera.transform.position);
            sceneCamera.transform.position = texelPos; //todo: best if we can just change camera offset

            var snapErrInPixels = ConvertWorldSpaceUnitToOverRenderPixels(snapErr);

            if (isCamSmoothing)
            {
                Shader.SetGlobalVector(PixelToMove, new Vector4((float)snapErrInPixels.x, (float)snapErrInPixels.y, 0, 0));
            }
            else
            {
                Shader.SetGlobalVector(PixelToMove, Vector4.zero);
            }
            
            SnapRequested?.Invoke();
        }

        private void EndCameraRendering(ScriptableRenderContext ctx, Camera renderCamera)
        {
            if (renderCamera != sceneCamera) return;

            RestoreTransformAfterRendering();
            
            UnsnapRequested?.Invoke();
        }

        private void SetTexture(RenderTexture renderTexture)
        {
            rtQuad.material.mainTexture = renderTexture;

            var targetResolution = pixelRenderFeature.GetTargetResolution();
            var quadScaleUnitPerResolutionUnit = rtQuad.transform.lossyScale.y / targetResolution.y; //still relies on setup...
            var targetCameraHeightWs = targetResolution.y * quadScaleUnitPerResolutionUnit;
            var orthoSize = targetCameraHeightWs / 2f;
            cameraForRenderTexture.orthographicSize = orthoSize;
        }

        private void CacheTransformBeforeRendering()
        {
            _cachedCameraPosition = sceneCamera.transform.position;
        }

        private void RestoreTransformAfterRendering()
        {
            sceneCamera.transform.position = _cachedCameraPosition;
        }

        public (Vector3 texelPos, Vector3 snapErr) FindClosestTexelPos(Vector3 position)
        {
            var texelSize = GetTexelSize();
            //Note: if creating a bunch of them every frame is a problem we can cache this?
            var trsForGridSpace = TRSForGridSpace();

            // Convert to local space of the camera's visual perspective.
            var localPos = trsForGridSpace.MultiplyPoint(position);

            // Snap XY to texel grid. Ignore z-axis for now that's - we probably don't need to handle it as zooming in orthographic camera works differently
            var snappedX = Mathf.Round(localPos.x / texelSize) * texelSize;
            var snappedY = Mathf.Round(localPos.y / texelSize) * texelSize;

            // Calculate rounding error - dropping z axis as we don't care about that in the quad space...
            var snapError = new Vector2(localPos.x - snappedX, localPos.y - snappedY);

            // Apply snapped position to pivot.
            var snappedPos = new Vector3(snappedX, snappedY, localPos.z);
            var texelPos = trsForGridSpace.inverse.MultiplyPoint(snappedPos); //todo:3x4 much faster - but no projection, is it really the thing that's wrong?
            var snapErrorWs = trsForGridSpace.inverse.MultiplyPoint(snapError);

            return (texelPos, snapErrorWs);
        }

        private Matrix4x4 TRSForGridSpace() => Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(sceneCamera.transform.rotation), Vector3.one);

        /// <summary>
        /// This is how many WS form one of our texels.
        /// </summary>
        private float GetTexelSize()
        {
            var pixelRenderHeight = pixelRenderFeature.GetDownsampleTargetSize().y;
            var cameraHeight = sceneCamera.orthographicSize * 2f;
            var texelSize = cameraHeight / pixelRenderHeight;
            return texelSize;
        }

        private double2 ConvertWorldSpaceUnitToOverRenderPixels(Vector3 worldSpace)
        {
            //todo: even if this works we need to make sure we dynamically resize the quad/camera size, it's not gonna be easy
            var cameraHeight = sceneCamera.orthographicSize * 2.0;
            var cameraWidth = cameraHeight * sceneCamera.aspect;
            var overRender = pixelRenderFeature.GetRenderSizeForSmoothPixelCamera();
            var pixelPerCameraXSpace = overRender.x / cameraWidth;
            var pixelPerCameraYSpace = overRender.y / cameraHeight;

            var trs = TRSForGridSpace();
            var localPos = trs.MultiplyPoint(worldSpace);

            // Dropping the z-axis, there's no z-axis in the canvas. 
            var quadSpace = new double2(localPos.x * pixelPerCameraXSpace, localPos.y * pixelPerCameraYSpace);
            return quadSpace;
        }

        /// <summary>
        /// Another way to think about this -> it's how many UV space in the RT is representing one pixel in the scene camera over render.
        /// </summary>
        /// <param name="sceneCameraUV"></param>
        /// <returns></returns>
        private double2 ConvertOverRenderUVToTargetResUV(double2 sceneCameraUV)
        {
            /*
             * This assume the RT is using a "remapped" UV so the "center" of what sceneCam is rendering is shown in the screen as a base.
             * Instead of trying to cram everything into the now undersized quad.
             */
            var overRenderSize = pixelRenderFeature.GetRenderSizeForSmoothPixelCamera();
            var pixelsInSceneCam = new double2(
                math.lerp(0, overRenderSize.x, sceneCameraUV.x),
                math.lerp(0, overRenderSize.y, sceneCameraUV.y)
            );

            var targetRes = pixelRenderFeature.GetTargetResolution();
            var rtCamUvToPixels = new double2(1.0 / overRenderSize.x, 1.0 / overRenderSize.y);


            // scaling *down* the UV
            var rtCameraUv = pixelsInSceneCam * rtCamUvToPixels;
            return rtCameraUv;
        }
    }
}
