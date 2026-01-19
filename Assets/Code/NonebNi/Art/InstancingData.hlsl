#ifndef INSTANCING_DATA_INCLUDED
#define INSTANCING_DATA_INCLUDED

struct InstanceData
{
    float4x4 trsMatrix; // we expect this to be relative to the object position.
};

StructuredBuffer<InstanceData> _PerInstanceData;

#endif
