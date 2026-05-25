using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace NonebNi.Art.Pixelation
{
    //todo: don't really understand how composite works - will need that to blend in outline
//todo: figure out why is it happening.
    /// <summary>
    /// Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs#L233
    /// </summary>
    public class UpscaleRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material upscaleMaterial = null!;

        private RenderPass _pass = null!;

        public override void Create()
        {
            _pass = new (upscaleMaterial)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType is CameraType.Game or CameraType.SceneView) renderer.EnqueuePass(_pass);
        }

        private class RenderPass : ScriptableRenderPass
        {
            private readonly Material _material;

            public RenderPass(Material material)
            {
                _material = material;
                requiresIntermediateTexture = true;

                // Requires color, depth, and normal buffers for 3D reconstruction during the Outline pass.
                ConfigureInput(ScriptableRenderPassInput.Color);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraTarget = resourceData.activeColorTexture;
                var source = ScriptableRenderPassUtils.GetSourceTexture(frameData);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Upsample", out var passData))
                {
                    passData.Source = source;
                    passData.Material = _material;

                    builder.UseTexture(passData.Source);
                    builder.SetRenderAttachment(cameraTarget, 0);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => ExecutePass(data, context));
                }
            }

            private static void ExecutePass(PassData data, RasterGraphContext context)
            {
                Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), data.Material, 0);
            }

            private class PassData
            {
                public TextureHandle Source;
                public Material? Material;
            }
        }
    }
}