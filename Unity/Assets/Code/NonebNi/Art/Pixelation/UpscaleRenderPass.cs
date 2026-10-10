using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace NonebNi.Art.Pixelation
{
    // https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/Runtime/2D/Rendergraph/UpscalePass.cs#L34

    //todo: don't really understand how composite works - will need that to blend in outline
//todo: figure out why is it happening.
    //todo: need to figure out the pixel conversion - also larger screen should mean render more things in camera, atm this approach is just making everything larger, which is bad.
    /// <summary>
    /// Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs#L233
    /// </summary>
    
    public class UpscaleRenderPass : ScriptableRenderPass
    {
        private readonly Settings _settings;

        public UpscaleRenderPass(Settings settings)
        {
            _settings = settings;
            requiresIntermediateTexture = true;

            // Requires color, depth, and normal buffers for 3D reconstruction during the Outline pass. PTO?
            ConfigureInput(ScriptableRenderPassInput.None);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraTarget = resourceData.activeColorTexture; //todo: psure it's up scaling to wrong res
            var source = ScriptableRenderPassUtils.GetSourceTexture(frameData);

            var blitParams = new RenderGraphUtils.BlitMaterialParameters(source, cameraTarget, _settings.upscaleMaterial, 0);
            renderGraph.AddBlitPass(blitParams);
            // using (var builder = renderGraph.AddRasterRenderPass<PassData>("Upsample", out var passData))
            // {
            //     passData.Source = source;
            //     passData.Material = _settings.upscaleMaterial;
            //
            //     builder.UseTexture(passData.Source);
            //     builder.SetRenderAttachment(cameraTarget, 0);
            //
            //     builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => ExecutePass(data, context));
            // }
        }

        private static void ExecutePass(PassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.Source, new (1, 1, 0, 0), data.Material, 0);
        }

        private class PassData
        {
            public TextureHandle Source;
            public Material Material = null!;
        }

        [Serializable]
        public class Settings
        {
            public Material upscaleMaterial = null!;        
            public Vector2Int baseResolution = new Vector2Int(1920, 1080);
        }
    }
}