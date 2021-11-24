
#ifndef TRUNC_INCLUDED
#define TRUNC_INCLUDED

float Trunc(float x, float levelNum)
{
    return floor(x * levelNum) / levelNum;
}

float2 Trunc2(float2 x, float2 levelNum)
{
    return floor(x * levelNum) / levelNum;
}

void Trunc_float(float x, float levelNum, out float y)
{
    y = Trunc(x, levelNum);
}

void Trunc2_float2(float2 x, float2 levelNum, out float2 y)
{
    y = Trunc2(x, levelNum);
}

#endif