// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)  

/*  
 * Note: * Unity's view/model matrix doesn't adapt to GPU instancing, * ordinarily if you are on shaders you can work around it using macros, * but as we are on SG land here we need to overwrite whatever value is on unity_ObjectToWorld and unity_WorldToObject.  
 * Otherwise all the graph's transformation matrix node won't work. * https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/CGIncludes/UnityStandardParticleInstancing.cginc 
 */

#ifndef NONEB_GPU_INSTANCING_SETUP_FOR_SHADER_GRAPH_INCLUDED
#define NONEB_GPU_INSTANCING_SETUP_FOR_SHADER_GRAPH_INCLUDED  

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
#include "InstancingData.hlsl"
#include "UnityInstancing.cginc"

void VertInstancingMatrices(InstanceData data, inout float4x4 objectToWorld, inout float4x4 worldToObject)
{
    // transform matrix - make sure we account for the object TRS.  
    objectToWorld = mul(objectToWorld, data.trsMatrix);

    /*
     * Note:
     * We are doing the expensive way to inverse the matrix. If perf is an issue look at this
     * https://discussions.unity.com/t/indirect-procedural-rendering-with-shader-graph/1664601/4
     * 
     * ``` Quote
     * Note that in the example above,
     * I use transpose(unity_ObjectToWorld) to set unity_WorldToObject which works given there’s no scaling.
     * If we need to handle scaling, then a proper inversed matrix is required, which is expensive to do in HLSL.
     * In such case it’s better to provide an inversed matrix from C#, or from a Compute shader, as to do it only one per instance.
     * ```
     */

    // inverse transform matrix  
    float3x3 worldToObjectRotation;
    worldToObjectRotation[0] = objectToWorld[1].yzx * objectToWorld[2].zxy - objectToWorld[1].zxy * objectToWorld[2].
        yzx;
    worldToObjectRotation[1] = objectToWorld[0].zxy * objectToWorld[2].yzx - objectToWorld[0].yzx * objectToWorld[2].
        zxy;
    worldToObjectRotation[2] = objectToWorld[0].yzx * objectToWorld[1].zxy - objectToWorld[0].zxy * objectToWorld[1].
        yzx;
    float det = dot(objectToWorld[0].xyz, worldToObjectRotation[0]);

    worldToObjectRotation = transpose(worldToObjectRotation);

    worldToObjectRotation *= rcp(det);

    float3 worldToObjectPosition = mul(worldToObjectRotation, -objectToWorld._14_24_34);

    worldToObject._11_21_31_41 = float4(worldToObjectRotation._11_21_31, 0.0f);
    worldToObject._12_22_32_42 = float4(worldToObjectRotation._12_22_32, 0.0f);
    worldToObject._13_23_33_43 = float4(worldToObjectRotation._13_23_33, 0.0f);
    worldToObject._14_24_34_44 = float4(worldToObjectPosition, 1.0f);
}

void VertInstancingSetup(InstanceData data)
{
    VertInstancingMatrices(data, unity_ObjectToWorld, unity_WorldToObject);
}

#endif
