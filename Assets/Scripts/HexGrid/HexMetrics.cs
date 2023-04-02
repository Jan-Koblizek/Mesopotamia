using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public static Texture2D noiseSource;

    public const float solidFactor = 0.9f;
    public const float blendFactor = 1f - solidFactor;
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;
    public const int chunkSizeX = 2, chunkSizeZ = 2;

    public const float cliffTreshold = 3;
    public const float maxNeighborHeightDifference = 4;
    public const float maxNeighborHeightDifferenceWater = 1;
    public const float maxElevation = 6;
    public const float minElevation = -4;

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * 0.003f, position.z * 0.003f);
    }

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };

    public static Vector3[] solidCorners = {
        new Vector3(0f, 0f, outerRadius * solidFactor),
        new Vector3(innerRadius * solidFactor, 0f, 0.5f * outerRadius * solidFactor),
        new Vector3(innerRadius * solidFactor, 0f, -0.5f * outerRadius * solidFactor),
        new Vector3(0f, 0f, -outerRadius * solidFactor),
        new Vector3(-innerRadius * solidFactor, 0f, -0.5f * outerRadius * solidFactor),
        new Vector3(-innerRadius * solidFactor, 0f, 0.5f * outerRadius * solidFactor)
    };
}