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
        float lacunarity)
    {
        var totalWorldSize = worldSize * chunkSize;

        var heightMap = new float[totalWorldSize, totalWorldSize];

        for (int y = 0; y < totalWorldSize; y++)
        {
            for (int x = 0; x < totalWorldSize; x++)
            {
                var xSamplePosition = ((float)x / (float)totalWorldSize) * worldScale;
                var ySamplePosition = ((float)y / (float)totalWorldSize) * worldScale;

                var perlinValue = Mathf.PerlinNoise(xSamplePosition, ySamplePosition);

                var noiseHeight = perlinValue * heightMultiplier;

                heightMap[x, y] = noiseHeight;
            }
        }

        return heightMap;
    }
}
