#ifndef GRASS_INSTANCED_INCLUDED
#define GRASS_INSTANCED_INCLUDED

#include "InstancingData.hlsl"
#include "GPUInstancingSetUpForShaderGraph.hlsl"

void Instancing_float(float3 pos, float instanceId, out float3 outPos, out float3 outNormal)
{
    InstanceData data = _PerInstanceData[instanceId];
    VertInstancingSetup(data);

    outPos = pos;
    //todo: normal feels wrong
    outNormal = mul(data.trsMatrix, float4(0., 1., 0., 0.)).xyz;
}

#endif
