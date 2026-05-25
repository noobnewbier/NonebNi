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

            // Receive the sub-texel offset calculated by the isometric camera.
            float4 _PixelPanOffset;

            half4 frag(Varyings input) : SV_Target
            {
                // Apply snap error compensation during texture sampling.
                float2 uv = input.texcoord + _PixelPanOffset.xy;

                float2 fw = clamp(fwidth(uv) / _BlitTexture_TexelSize.xy, 1e-5, 1.0);
                float2 grid = uv / _BlitTexture_TexelSize.xy - 0.5 * fw;
                float2 blend = smoothstep(1.0 - fw, float2(1.0, 1.0), frac(grid));
                float2 finalUV = (floor(grid) + 0.5 + blend) * _BlitTexture_TexelSize.xy;

                half4 color = SAMPLE_TEXTURE2D_LOD(_BlitTexture, sampler_LinearClamp, finalUV, 0);

                return color;
            }
            ENDHLSL
        }
    }
}