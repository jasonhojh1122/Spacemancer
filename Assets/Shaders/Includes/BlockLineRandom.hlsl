
#ifndef BLOCK_LINE_RANDOM_INCLUDED
#define BLOCK_LINE_RANDOM_INCLUDED

#include "Noise.hlsl"

float Trunc(float x, float levelNum)
{
    return floor(x * levelNum) / levelNum;
}

float2 Trunc2(float2 x, float2 levelNum)
{
    return floor(x * levelNum) / levelNum;
}

void UVOffsetX_float(float glitchFreq, float splitCnt, float time, float2 uv, out float xOffset)
{

    float strength = 0.5 + 0.5 * cos(time * glitchFreq);
    float seed = time * strength;
    float cosTrunc = Trunc(seed, 4);

    xOffset = cosTrunc;

    float randSplitCnt = Hash2( Trunc2(float2(uv.y, uv.y), float2(splitCnt, splitCnt)) + 100.0 * cosTrunc );
    float lineOffset = 6.0 * Trunc(seed, 24.0 * randSplitCnt);
    // lineOffset = clamp(lineOffset, -1.0, 1.0);

    xOffset = 0.5 * Hash2( Trunc2(float2(uv.y, uv.y) + lineOffset, float2(splitCnt, splitCnt)) );
    xOffset += 0.5 * Hash2( Trunc2(float2(uv.y, uv.y) + lineOffset, float2(splitCnt-1.0, splitCnt-1.0)) );
    xOffset = 2.0 * xOffset - 1.0;
    xOffset = clamp(xOffset, -1.0, 1.0);

    // newUV = saturate(uv + float2(0.1 * uvOffset, 0.0));

}

#endif