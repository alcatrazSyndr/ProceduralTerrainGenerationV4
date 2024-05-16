using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoiseHeightMap
{
    public static float[,] GeneratePerlinNoiseHeightMap(
        int worldSize,
        int chunkSize,
        float worldScale,
        float heightMultiplier,
        int octaves,
        float persistence,
        float lacunarity,
        AnimationCurve heightMapCurve,
        bool falloff,
        bool invertFalloff,
        float falloffMainlandSize,
        float falloffTransitionWidth)
    {
        var totalWorldSize = worldSize * chunkSize;

        var heightMap = new float[totalWorldSize, totalWorldSize];

        for (int y = 0; y < totalWorldSize; y++)
        {
            for (int x = 0; x < totalWorldSize; x++)
            {
                var amplitude = 1f;
                var frequency = 1f;

                var noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    var xSamplePosition = ((float)x / (float)totalWorldSize) * worldScale * frequency;
                    var ySamplePosition = ((float)y / (float)totalWorldSize) * worldScale * frequency;

                    var perlinValue = Mathf.PerlinNoise(xSamplePosition, ySamplePosition);

                    noiseHeight += (perlinValue * amplitude);

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (falloff)
                {
                    var falloffValue = Falloff.EvaluateWorldFalloffMap(x, y, totalWorldSize, falloffMainlandSize, falloffTransitionWidth);

                    noiseHeight -= falloffValue;
                }

                heightMap[x, y] = Mathf.Clamp01(heightMapCurve.Evaluate(noiseHeight));
            }
        }

        return heightMap;
    }
}
