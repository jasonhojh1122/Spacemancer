// https://github.com/QianMo/X-PostProcessing-Library/blob/master/Assets/X-PostProcessing/Effects/GlitchImageBlock/Shader/GlitchImageBlock.shader

#ifndef BLOCKNOISE_INCLUDED
#define BLOCKNOISE_INCLUDED

void BlockNoise_float(float2 seed, float time, out float noise)
{
    noise = frac(sin(dot(seed * floor(time * 30.0), float2(127.1, 311.7))) * 43758.5453123);
}

#endif