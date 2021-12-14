
#ifndef SELECTCOLOR_INCLUDED
#define SELECTCOLOR_INCLUDED

bool CloseFloat(float f1, float f2)
{
    return abs(f1-f2) < 0.01;
}

void SelectColor_float(float key, float3 blue, float3 green, float3 red, float3 cyan,
    float3 magenta, float3 yellow, float3 white, float3 black, out float3 outColor)
{
        if (CloseFloat(key, 1.0))
            outColor = blue;
        else if (CloseFloat(key, 2.0))
            outColor = green;
        else if (CloseFloat(key, 4.0))
            outColor = red;
        else if (CloseFloat(key, 3.0))
            outColor = cyan;
        else if (CloseFloat(key, 5.0))
            outColor = magenta;
        else if (CloseFloat(key, 6.0))
            outColor = yellow;
        else if (CloseFloat(key, 7.0))
            outColor = white;
        else if (CloseFloat(key, 8.0))
            outColor = black;
        else
            outColor = white;
}

void IsInRange_float(float dist, float startDist, float endDist, out bool isInRange)
{
    isInRange = ((dist >= startDist) && (dist <= endDist));
}

#endif