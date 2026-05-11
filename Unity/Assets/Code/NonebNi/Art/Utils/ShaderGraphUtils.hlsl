#ifndef NONEB_SHADER_GRAPH_UTILS
#define NONEB_SHADER_GRAPH_UTILS  

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
#include "UnityInstancing.cginc"

void RoundToNearestMultiple_float(float input, float n, out float output)
{
    output = floor(input / n) * n;
}

#endif
