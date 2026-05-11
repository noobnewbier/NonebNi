using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace NonebNi.Art.Pixelation
{
    public static class ScriptableRenderPassUtils
    {
        public static TextureHandle GetSourceTexture(ContextContainer frameData)
        {
            if (frameData.Contains<TextureData>())
            {
                var textureData = frameData.Get<TextureData>();
                return textureData.Handle;
            }

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraTarget = resourceData.activeColorTexture;
            return cameraTarget;
        }
    }
}