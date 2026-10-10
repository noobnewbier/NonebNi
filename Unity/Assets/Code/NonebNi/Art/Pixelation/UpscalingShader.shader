/*
Original:
https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/5786325b7bd6359a580d8b98210443026bb7eddf/Assets/Shaders/PostProcess/SharpUpscaleShader.shader

Tech Reference:
https://jorenjoestar.github.io/post/pixel_art_filtering/
https://www.youtube.com/watch?v=d6tp43wZqps
https://colececil.dev/blog/2017/scaling-pixel-art-without-destroying-it/
https://www.davidhol.land/articles/3d-pixel-art-rendering/
*/

Shader "NonebNi/SharpUpscale"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = float2(input.texcoord.x, input.texcoord.y);
                
                float2 boxSize = clamp(fwidth(uv) * _BlitTexture_TexelSize.zw, 1e-5, 1); // todo: immediate thought box size mismatch...?
                float2 tx = uv * _BlitTexture_TexelSize.zw - .5 * boxSize;
                float2 txOffset = smoothstep(1 - boxSize, 1, frac(tx));
                float2 finalUV = (floor(tx) + .5 + txOffset) * _BlitTexture_TexelSize.xy;
                
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, finalUV);
                return color;
            }
            ENDHLSL
        }
    }
}