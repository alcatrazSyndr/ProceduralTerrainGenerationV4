using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldChunkMeshGenerator
{
    public static Mesh GenerateWorldChunkMesh(float[,] heightMap)
    {
        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);

        var meshData = new WorldChunkMeshData(width, height);
        var vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.Vertices[vertexIndex] = new Vector3(x, heightMap[x, y], y);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + width);
                }

                vertexIndex++;
            }
        }

        return meshData.CreateMesh();
    }
}
