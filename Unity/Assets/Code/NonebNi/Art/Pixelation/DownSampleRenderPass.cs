using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;


namespace NonebNi.Art.Pixelation
{
    /// <summary>
    /// Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs#L233
    /// </summary>
    public class DownSampleRenderPass : ScriptableRenderPass
    {
        private readonly Settings _settings;

        public DownSampleRenderPass(Settings settings)
        {
            _settings = settings;
            requiresIntermediateTexture = true;

            ConfigureInput(ScriptableRenderPassInput.None);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraTarget = resourceData.activeColorTexture;

            if (!cameraTarget.IsValid()) return;

            var cameraData = frameData.Get<UniversalCameraData>();

            // Low resolution scene color using Bilinear filtering to ensure Sharp Upscale works correctly. TODO: WHY? why bilinear filtering?
            var lowResDesc = new TextureDesc(_settings.RenderSize.x, _settings.RenderSize.y)
            {
                colorFormat = cameraData.cameraTargetDescriptor.graphicsFormat,
                depthBufferBits = DepthBits.None,
                useMipMap = false,
                filterMode = FilterMode.Point,
                name = "_LowResColor",
            };
            var lowResColor = renderGraph.CreateTexture(lowResDesc);

            // // Create a BlitMaterialParameters object with the blit material, source texture, destination texture, and shader pass to use.
            // var blitParams = new RenderGraphUtils.BlitMaterialParameters(cameraTarget, lowResColor, null, 0);
            // renderGraph.AddBlitPass(blitParams, "Downsample Blit");
            //
            // var textureData = frameData.GetOrCreate<TextureData>();
            // textureData.Handle = lowResColor;

            // Downsample color to internal resolution. //todo: might be funky creating texture.
            // var downscaleMat = _settings.DownsampleMat;
            // if (downscaleMat == null)
            // {
            //     downscaleMat = Blitter.GetBlitMaterial(TextureDimension.Tex2D);
            // }
            // var blitParams = new RenderGraphUtils.BlitMaterialParameters(cameraTarget, lowResColor, downscaleMat, 0);
            // renderGraph.AddBlitPass(blitParams);
            //     var textureData = frameData.GetOrCreate<TextureData>();
            //     textureData.Handle = lowResColor;


            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Downsample_Color", out var passData))
            {
                passData.Source = cameraTarget;
                passData.Destination = lowResColor;

                builder.UseTexture(passData.Source);
                builder.SetRenderAttachment(passData.Destination, 0);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => ExecutePass(data, context));
                var textureData = frameData.GetOrCreate<TextureData>();
                textureData.Handle = lowResColor;

                builder.SetGlobalTextureAfterPass(lowResColor, Shader.PropertyToID("_LowResColor"));
                
                builder.AllowPassCulling(false);
            }
        }

        private static void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), 0, false);
        }

        private class PassData
        {
            public TextureHandle Destination;
            public TextureHandle Source;
        }

        [Serializable]
        public class Settings
        {
            public Vector2Int RenderSize { get; private set; }
            public Material DownsampleMat { get; private set; }

            public Settings(Vector2Int renderRenderSize, Material downsampleMat)
            {
                RenderSize = renderRenderSize;
                DownsampleMat = downsampleMat;
            }
        }
    }
}