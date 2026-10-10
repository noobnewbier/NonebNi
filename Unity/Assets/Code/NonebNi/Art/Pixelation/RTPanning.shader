Shader "NonebNi/RTPanning"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)

    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        // -------------------------------------
        // Render State Commands
        Cull Off ZWrite On ZTest Always

        Pass
        {
            Name "RT Panning"

            // -------------------------------------
            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex UnlitPassVertex
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"
            
            // Receive the sub-texel offset calculated by the isometric camera.
            float4 _OverRenderSize = float4(1923, 1083, 0,0);
            float4 _TargetResolution = float4(1920, 1080, 0, 0);
            float4 _PixelToMove;

            float4 frag(Varyings input) : SV_Target
            {
                // Remap the UV to the center of the texture.
                float2 overRenderPixelPaddingToCenter = (_OverRenderSize.xy - _TargetResolution.xy) / 2.;
                float2 targetResPixelPerUv = float2(1., 1.) / _TargetResolution.xy;
                float2 targetResUVPaddingPerSide = overRenderPixelPaddingToCenter * targetResPixelPerUv;
                
                float2 uv = float2(
                    lerp(targetResUVPaddingPerSide.x, 1 - targetResUVPaddingPerSide.x, input.uv.x),
                    lerp(targetResUVPaddingPerSide.y, 1 - targetResUVPaddingPerSide.y, input.uv.y)
                );
                
                uv += _PixelToMove.xy * targetResPixelPerUv;
                
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_PointClamp, uv);
                return color;
            }
            
            ENDHLSL
        }

    }
}