using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace NonebNi.Art.Pixelation
{
    public class TextureData : ContextItem
    {
        public TextureHandle Handle;
        
        public override void Reset()
        {
            Handle = TextureHandle.nullHandle;
        }
    }
}