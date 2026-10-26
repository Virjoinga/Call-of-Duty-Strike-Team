#ifndef CORONA_LEGACY_INCLUDED
#define CORONA_LEGACY_INCLUDED

#include "UnityCG.cginc"

// The recovered GLES programs calculate fog, depth bands and bias in a
// [-w, w] clip-depth range. Keep that arithmetic, then convert only the
// rasterizer position to the graphics API's native depth convention.
inline float4 CoronaObjectToClipPos(float4 vertex)
{
    float4 position = UnityObjectToClipPos(vertex);
#if defined(UNITY_REVERSED_Z)
    position.z = position.w - 2.0 * position.z;
#elif !defined(SHADER_API_GLCORE) && !defined(SHADER_API_GLES) && !defined(SHADER_API_GLES3)
    position.z = 2.0 * position.z - position.w;
#endif
    return position;
}

inline float4 CoronaClipToNative(float4 position)
{
#if defined(UNITY_REVERSED_Z)
    position.z = (position.w - position.z) * 0.5;
#elif !defined(SHADER_API_GLCORE) && !defined(SHADER_API_GLES) && !defined(SHADER_API_GLES3)
    position.z = (position.z + position.w) * 0.5;
#endif
    return position;
}

#endif
