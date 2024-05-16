using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    public Vector2Int ChunkCoordinate;
    public Vector3 ChunkPosition;
    public GameObject ChunkGO;
    public MeshFilter ChunkMeshFilter;
    public MeshRenderer ChunkMeshRenderer;
    public MeshCollider ChunkMeshCollider;

    public WorldChunk(Vector2Int worldCoordinate, float[,] heightMap, int chunkSize, Material chunkMat, float heightMultiplier)
    {
        ChunkGO = new GameObject("WorldChunk_" + worldCoordinate.ToString());

        ChunkMeshFilter = ChunkGO.AddComponent<MeshFilter>();
        var mesh = WorldChunkMeshGenerator.GenerateWorldChunkMesh(heightMap, heightMultiplier);
        ChunkMeshFilter.mesh = mesh;

        ChunkMeshRenderer = ChunkGO.AddComponent<MeshRenderer>();
        ChunkMeshRenderer.material = chunkMat;

        ChunkMeshCollider = ChunkGO.AddComponent<MeshCollider>();

        ChunkPosition = new Vector3(worldCoordinate.x, 0f, worldCoordinate.y) * (float)chunkSize;
        ChunkGO.transform.position = ChunkPosition;
    }
}
