
#ifndef COLORNOISE_INCLUDED
#define COLORNOISE_INCLUDED

#include "Noise.hlsl"

void ColorNoise_float(float2 seed, float time, out float3 noise)
{
    Noise2_float(seed+float2(10.15,14.89), time, noise.x);
    Noise2_float(seed+float2(27.92,13.73), time, noise.y);
    Noise2_float(seed+float2(54.19,23.18), time, noise.z);
    noise = noise - 0.5;
}

#endif