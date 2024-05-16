using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkController : MonoBehaviour
{
    [Header("World Generation Data")]
    [SerializeField] private int _worldSize = 1;
    [SerializeField] private int _chunkSize = 240;
    [SerializeField] private float _worldScale = 1f;
    [SerializeField] private float _heightMultiplier = 1f;
    [SerializeField] private int _octaves = 1;
    [SerializeField] private float _persistence = 1f;
    [SerializeField] private float _lacunarity = 1f;
    [SerializeField] private Material _worldChunkMaterial;

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        var worldHeightMap = PerlinNoiseHeightMap.GeneratePerlinNoiseHeightMap(_worldSize, _chunkSize, _worldScale, _heightMultiplier, _octaves, _persistence, _lacunarity);
        var worldHeightMapDictionary = GenerateChunkDictionaryFromWorldHeightMap(worldHeightMap);

        for (int x = 0; x < _worldSize; x++)
        {
            for (int y = 0; y < _worldSize; y++)
            {
                var worldCoordinate = new Vector2Int(x, y);
                var worldChunkHeightMap = worldHeightMapDictionary[worldCoordinate];
                var worldChunk = new WorldChunk(worldCoordinate, worldChunkHeightMap, _chunkSize, _worldChunkMaterial);

                worldChunk.ChunkGO.transform.SetParent(transform);
            }
        }
    }

    private Dictionary<Vector2Int, float[,]> GenerateChunkDictionaryFromWorldHeightMap(float[,] worldHeightMap)
    {
        var worldChunkDictionary = new Dictionary<Vector2Int, float[,]>();

        for (int yWorld = 0; yWorld < _worldSize; yWorld++)
        {
            for (int xWorld = 0; xWorld < _worldSize; xWorld++)
            {
                var worldChunkCoordinate = new Vector2Int(xWorld, yWorld);

                var xEdgeModifier = (xWorld == _worldSize - 1) ? 0 : 1;
                var yEdgeModifier = (yWorld == _worldSize - 1) ? 0 : 1;

                var xWorldPosition = xWorld * _chunkSize;
                var yWorldPosition = yWorld * _chunkSize;

                var worldChunkHeightMap = new float[_chunkSize + xEdgeModifier, _chunkSize + yEdgeModifier];

                for (int yChunk = 0; yChunk < _chunkSize + yEdgeModifier; yChunk++)
                {
                    for (int xChunk = 0; xChunk < _chunkSize + xEdgeModifier; xChunk++)
                    {
                        worldChunkHeightMap[xChunk, yChunk] = worldHeightMap[xWorldPosition + xChunk, yWorldPosition + yChunk];
                    }
                }

                worldChunkDictionary.Add(worldChunkCoordinate, worldChunkHeightMap);
            }
        }

        return worldChunkDictionary;
    }
}
