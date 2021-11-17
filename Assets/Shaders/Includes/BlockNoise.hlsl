// https://github.com/QianMo/X-PostProcessing-Library/blob/master/Assets/X-PostProcessing/Effects/GlitchImageBlock/Shader/GlitchImageBlock.shader

#ifndef BLOCKNOISE_INCLUDED
#define BLOCKNOISE_INCLUDED

void Noise3_float(float3 seed, float time, out float noise)
{
    float2 n1;
    n1.xy = Noise2_float(seed.xy);
    n1.z = Noise1_float(seed.z);
}

void Noise2_float(float2 seed, float time, out float noise)
{
    noise = frac(sin(dot(seed * floor(time * 30.0), float2(127.1, 311.7))) * 43758.5453123);
}

void Noise_float(float seed, float time, out float noise)
{
    noise = Noise2_float(seed, time);
}

#endif