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
    public class PixelRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private PixelSettings settings = new ();
        private PixelRenderPass _pixelPass = null!;

        public override void Create()
        {
            _pixelPass = new (settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType is CameraType.Game or CameraType.SceneView) renderer.EnqueuePass(_pixelPass);
        }

        [Serializable]
        public class PixelSettings
        {
            [Header("Internal Resolution")]
            public int width = 640;
            public int height = 360;
        }

        private class PixelRenderPass : ScriptableRenderPass
        {
            private readonly PixelSettings _settings;

            public PixelRenderPass(PixelSettings settings)
            {
                _settings = settings;
                requiresIntermediateTexture = true;

                // Requires color, depth, and normal buffers for 3D reconstruction during the Outline pass.
                ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraTarget = resourceData.activeColorTexture;

                if (!cameraTarget.IsValid()) return;

                var cameraData = frameData.Get<UniversalCameraData>();

                // Texture and Descriptor Creation

                // Backup full resolution color to prevent read/write conflicts.
                var colorCopyDesc = renderGraph.GetTextureDesc(cameraTarget);
                colorCopyDesc.name = "_ColorCopy";
                colorCopyDesc.msaaSamples = MSAASamples.None;
                colorCopyDesc.clearBuffer = false;
                var colorCopy = renderGraph.CreateTexture(colorCopyDesc);

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

                // Pass 0: Capture the original full resolution color.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("CopyColor", out var passData))
                {
                    passData.Source = cameraTarget;
                    passData.Destination = colorCopy;

                    builder.UseTexture(passData.Source);
                    builder.SetRenderAttachment(passData.Destination, 0);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), 0, false));
                }

                // Pass 1: Downsample color to internal resolution.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Downsample_Color", out var passData))
                {
                    passData.Source = colorCopy;
                    passData.Destination = lowResColor;

                    builder.UseTexture(passData.Source);
                    builder.SetRenderAttachment(passData.Destination, 0);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), 0, false));
                }
                
                // Upscaling... without the fwidth thing? TODO: we will need to do the fwidth thing. There's a good reason behind it.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Upsample_Color", out var passData))
                {
                    passData.Source = lowResColor;
                    passData.Destination = cameraTarget;

                    builder.UseTexture(passData.Source);
                    builder.SetRenderAttachment(passData.Destination, 0);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), 0, false));
                }

                //todo: another feature?
                // // Pass 4: Upscale to full resolution.
                // using (var builder = renderGraph.AddRasterRenderPass<PassData>("Upsample_Sharp", out var passData))
                // {
                //     passData.Source = lowResComposite;
                //     passData.Destination = cameraTarget;
                //     passData.Material = _settings.upscaleMaterial;
                //
                //     builder.UseTexture(passData.Source);
                //     builder.SetRenderAttachment(passData.Destination, 0);
                //
                //     builder.SetRenderFunc
                //     (
                //         (PassData data, RasterGraphContext context) =>
                //         {
                //             if (data.Material != null)
                //                 Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), data.Material, 0);
                //             else
                //                 Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), 0, false);
                //         }
                //     );
                // }
            }

            private class PassData
            {
                public TextureHandle Destination;
                public TextureHandle Source;
            }
        }
    }
}