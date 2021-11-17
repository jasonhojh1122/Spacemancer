// https://github.com/QianMo/X-PostProcessing-Library/blob/master/Assets/X-PostProcessing/Effects/GlitchImageBlock/Shader/GlitchImageBlock.shader

#ifndef NOISE_INCLUDED
#define NOISE_INCLUDED

void Noise2_float(float2 seed, float time, out float noise)
{
    noise = frac(sin(dot(seed * floor(time * 30.0), float2(127.1, 311.7))) * 43758.5453123);
}

void Noise_float(float seed, float time, out float noise)
{
    Noise2_float(seed, time, noise);
}

void Noise3_float(float3 seed, float time, out float noise)
{
    float2 n1;
    Noise2_float(seed.xy, time, n1.x);
    Noise_float(seed.z, time, n1.y);
    Noise2_float(n1, time, noise);
}

#endif