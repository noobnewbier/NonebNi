#ifndef GRASS_INSTANCED_INCLUDED
#define GRASS_INSTANCED_INCLUDED

#include "InstancingData.hlsl"
#include "GPUInstancingSetUpForShaderGraph.hlsl"

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#endif


/*
 * Note:
 * most likely necessary for shadows stuffs
 * https://github.com/Cyanilux/URP_ShaderGraphCustomLighting/blob/19fac6a6126a9c25d8f226b38bb12c7360557ff6/CustomLighting.hlsl#L12
 */
#ifndef SHADERGRAPH_PREVIEW
#if SHADERPASS != SHADERPASS_FORWARD && SHADERPASS != SHADERPASS_GBUFFER
// #if to avoid "duplicate keyword" warnings if this is included in a Lit Graph

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _CLUSTER_LIGHT_LOOP

// Left some keywords (e.g. light layers, cookies) in subgraphs to help avoid unnecessary shader variants
// But means if those subgraphs are nested in another, you'll need to copy the keywords from blackboard

#endif
#endif

float inverseLerp(float from, float to, float value)
{
    float interpolate = (value - from) / (to - from);
    interpolate = saturate(interpolate);

    return interpolate;
}

void Instancing_float(float3 pos, float instanceId, out float3 outPos, out float3 outNormal, out float3 outInstanceRootPos)
{
    InstanceData data = _PerInstanceData[instanceId];
    VertInstancingSetup(data);

    outPos = pos;
    //todo: normal feels wrong
    outNormal = mul(data.trsMatrix, float4(0., 1., 0., 0.)).xyz;
    outInstanceRootPos = data.trsMatrix._14_24_34;
}


void ShadowColor_float(float3 baseColor,
                       float featherRange,
                       float3 firstShadeColor,
                       float firstShadeStep,
                       float3 secondShadeColor,
                       float secondShadeStep,
                       float3 normalWs,
                       float3 positionWs,
                       out float3 finalColor)
{
    float3 lightDirection = normalize(float3(1, 1, -0.4));
    float3 lightColor = float3(1, 1, 1);
    float lightAttenuation = 1;

    #ifndef SHADERGRAPH_PREVIEW

    float4 shadowCoord = TransformWorldToShadowCoord(positionWs);
    half4 shadowMask = half4(1, 1, 1, 1);
    Light mainLight = GetMainLight(shadowCoord, positionWs, shadowMask);
    lightAttenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
    lightColor = mainLight.color;
    lightDirection = mainLight.direction;

    #endif

    float shadowStrength = 1. - lightAttenuation;
    float shadeRange = inverseLerp(firstShadeStep, secondShadeStep, shadowStrength);
    float isSecondShade = shadeRange >= 1;
    float3 shadeColor = lerp(firstShadeColor, secondShadeColor, isSecondShade);
    float isShaded = step(firstShadeStep, shadowStrength);

    float3 diffuse = lerp(baseColor, shadeColor, isShaded);
    
    float nDotL = saturate(dot(normalWs, lightDirection));
    float halfLambert = pow(nDotL * 0.5 + 0.5, 2.);
    float3 halfLambertDiffuse =  diffuse * (lightColor * halfLambert);

    finalColor = halfLambertDiffuse;
}

#endif
