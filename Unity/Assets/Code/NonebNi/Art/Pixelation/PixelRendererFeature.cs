using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NonebNi.Art.Pixelation
{
    /// <summary>
    /// There is four resolution we need to keep in mind:
    /// 1. The target resolution - this is the user display
    /// 2. The target resolution + the offset to allow for camera snapping - this is what's under the hood, behind the user display
    /// 3. The lowered sample resolution - this is downsampled target resolution, for creating the pixelated effect
    /// 4. The lowered sample resolution + 1 on both axis - this is what our render texture is at. 
    /// 
    /// Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs#L233
    /// </summary>
    public class PixelRendererFeature : ScriptableRendererFeature
    {
        /// <summary>
        /// How many pixels on both x/y dimension are downscaled to one pixel.
        /// </summary>
        [field: SerializeField, Min(1)] public int DownscaleStrength { get; private set; }
        [SerializeField] private Material downscaleMaterial;
        [SerializeField] private UpscaleRenderPass.Settings upscaleSettings = null!;

        private DownSampleRenderPass _downsamplePass = null!;
        private UpscaleRenderPass _upscalePass = null!;

        public override void Create()
        {
            var downsampleSetting = new DownSampleRenderPass.Settings(GetDownsampleTargetSize(), downscaleMaterial);
            _downsamplePass = new (downsampleSetting)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques,
            };
            _upscalePass = new (upscaleSettings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType is CameraType.Game or CameraType.SceneView) renderer.EnqueuePass(_downsamplePass);
            if (renderingData.cameraData.cameraType is CameraType.Game or CameraType.SceneView) renderer.EnqueuePass(_upscalePass);
        }

        public Vector2Int GetDownsampleTargetSize()
        {
            var finalRenderSize = GetRenderSizeForSmoothPixelCamera();
            return new (finalRenderSize.x / DownscaleStrength, finalRenderSize.y / DownscaleStrength);
        }

        public Vector2Int GetRenderSizeForSmoothPixelCamera()
        {
            //todo: if texelSize is not int we are kinda fucked here no?

            // create RT for our final target
            var maxShiftInPixelSpace = DownscaleStrength; // It's also how many pixels per axis, per texels.
            Vector2Int targetResolution = GetTargetResolution();
            var finalRenderTextureSize = new Vector2Int(targetResolution.x + maxShiftInPixelSpace, targetResolution.y + maxShiftInPixelSpace);

            return finalRenderTextureSize;
        }
        
        public Vector2Int GetBaseResolution() => new (upscaleSettings.baseResolution.x, upscaleSettings.baseResolution.y);

        //todo: maintain consistency across different screen size.
        public Vector2Int GetTargetResolution() => new (Screen.width, Screen.height);
    }
}