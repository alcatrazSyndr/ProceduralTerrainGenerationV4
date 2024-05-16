using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkMeshData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;

    private int _currentTriangleIndex;

    public WorldChunkMeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[_currentTriangleIndex] = a;
        Triangles[_currentTriangleIndex + 1] = b;
        Triangles[_currentTriangleIndex + 2] = c;

        _currentTriangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh();

        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;

        mesh.RecalculateNormals();

        return mesh;
    }
}
