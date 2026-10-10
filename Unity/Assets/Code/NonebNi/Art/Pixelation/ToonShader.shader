Shader "PTo/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _ShadedColor ("ShadedColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct appdata
			{
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
			};

			struct v2f
			{
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
			};

			sampler2D _MainTex;
			 float4 _Color;
				float4 _ShadedColor;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
                v2f OUT;
                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                OUT.viewDirWS = normalize(GetCameraPositionWS() - posWS);
                return OUT;
			}
			
			float4 frag (v2f i ) : SV_Target
			{
				// sample the texture
				
                // Simple toon shading (1 directional light)
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(i.normalWS, normalize(mainLight.direction)));
				
				float4 texCol = tex2D(_MainTex, i.uv);
                float toonStep = step(0.5, NdotL); // two-tone shading
				if (toonStep == 0)
				{
				  texCol *= _ShadedColor;
				}else {
					texCol *= _Color;
				}
				
                float3 litColor = texCol.rgb * mainLight.color.rgb;
				
				return float4(litColor, 1);
			}
			ENDHLSL
        }
    }
}