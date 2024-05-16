using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldChunkController))]
public class WorldChunkControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldChunkController myScript = (WorldChunkController)target;
        if (GUILayout.Button("Regenerate World"))
        {
            myScript.RegenerateWorld();
        }
    }
}

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
    [SerializeField] private bool _falloff = false;
    [SerializeField] private bool _invertFalloff = false;
    [SerializeField] private float _falloffMainlandSize = 1f;
    [SerializeField] private float _falloffTransitionWidth = 1f;
    [SerializeField] private Material _worldChunkMaterial;

    [Header("Runtime")]
    [SerializeField] private List<GameObject> _worldChunkGameObjectList = new List<GameObject>();

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        _worldChunkGameObjectList.Clear();

        var worldHeightMap = PerlinNoiseHeightMap.GeneratePerlinNoiseHeightMap(
            _worldSize, 
            _chunkSize, 
            _worldScale, 
            _heightMultiplier, 
            _octaves, 
            _persistence, 
            _lacunarity, 
            _falloff, 
            _invertFalloff, 
            _falloffMainlandSize, 
            _falloffTransitionWidth);
        var worldHeightMapDictionary = GenerateChunkDictionaryFromWorldHeightMap(worldHeightMap);

        for (int x = 0; x < _worldSize; x++)
        {
            for (int y = 0; y < _worldSize; y++)
            {
                var worldCoordinate = new Vector2Int(x, y);
                var worldChunkHeightMap = worldHeightMapDictionary[worldCoordinate];
                var worldChunk = new WorldChunk(worldCoordinate, worldChunkHeightMap, _chunkSize, _worldChunkMaterial);

                worldChunk.ChunkGO.transform.SetParent(transform);

                _worldChunkGameObjectList.Add(worldChunk.ChunkGO);
            }
        }
    }

    public void RegenerateWorld()
    {
        for (int i = _worldChunkGameObjectList.Count - 1; i >= 0; i--)
        {
            var worldChunkGO = _worldChunkGameObjectList[i];
            _worldChunkGameObjectList.RemoveAt(i);
            Destroy(worldChunkGO);
        }

        GenerateWorld();
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
