using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace NonebNi.Art.Pixelation
{
    //todo: don't really understand how composite works - will need that to blend in outline
    /// <summary>
    /// Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs#L233
    /// </summary>
    public class DownSampleRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private Settings settings = new ();
        private RenderPass _downsamplePass = null!;

        public override void Create()
        {
            _downsamplePass = new (settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType is CameraType.Game or CameraType.SceneView) renderer.EnqueuePass(_downsamplePass);
        }

        [Serializable]
        private class Settings
        {
            public int width = 640;
            public int height = 360;
        }

        private class RenderPass : ScriptableRenderPass
        {
            private readonly Settings _settings;

            public RenderPass(Settings settings)
            {
                _settings = settings;
                requiresIntermediateTexture = true;

                ConfigureInput(ScriptableRenderPassInput.Color);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraTarget = resourceData.activeColorTexture;

                if (!cameraTarget.IsValid()) return;

                var cameraData = frameData.Get<UniversalCameraData>();

                // Low resolution scene color using Bilinear filtering to ensure Sharp Upscale works correctly.
                var lowResDesc = new TextureDesc(_settings.width, _settings.height)
                {
                    colorFormat = cameraData.cameraTargetDescriptor.graphicsFormat,
                    depthBufferBits = DepthBits.None,
                    useMipMap = false,
                    filterMode = FilterMode.Bilinear,
                    name = "_LowResColor"
                };
                var lowResColor = renderGraph.CreateTexture(lowResDesc);
                
                // Downsample color to internal resolution.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Downsample_Color", out var passData))
                {
                    passData.Source = cameraTarget;
                    passData.Destination = lowResColor;

                    builder.UseTexture(passData.Source);
                    builder.SetRenderAttachment(passData.Destination, 0);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => ExecutePass(data, context));

                    //TODO: just pass data here
                    var textureData = frameData.GetOrCreate<TextureData>();
                    textureData.Handle = passData.Destination;
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
        }
    }
}