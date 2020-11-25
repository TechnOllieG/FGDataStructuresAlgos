using System;
using System.Collections;
using System.Collections.Generic;
using Freya;
using FutureGames.Lib;
using UnityEngine;

public struct Chunk
{
    public Vector2Int chunkCoordinates;
    public GameObject chunkObjects; // Parent of all objects in this chunk

    public Chunk(Vector2Int chunkCoordinates, GameObject chunkObjects)
    {
        this.chunkCoordinates = chunkCoordinates;
        this.chunkObjects = chunkObjects;
    }
}

public class WorldGenerator : MonoBehaviour
{
    public GameObject blockToSpawn;
    public GameObject player;
    public Vector2Int maxWorldSizeInChunks;
    public int renderDistance = 6;
    public float terrainScale = 1f;
    public float scale = 1f;
    public int octaves = 1;
    public float persistance = 1f;
    public float lacunarity = 1f;

    private int _chunkDimensions = 16;
    private List<Chunk> _renderedChunks = new List<Chunk>();
    private Vector2Int _minChunkValue;
    private Vector2Int _maxChunkValue;
    private Vector2Int _previousChunkCoordinate;

    private void OnValidate()
    {
        if (maxWorldSizeInChunks.x % 2 != 0) // if max size is uneven make it even
            maxWorldSizeInChunks.x -= 1;
        
        if (maxWorldSizeInChunks.y % 2 != 0)
            maxWorldSizeInChunks.y -= 1;
    }

    private void Awake()
    {
        Vector2Int halfSizeOfWorld = maxWorldSizeInChunks / 2;
        _minChunkValue = -halfSizeOfWorld;
        _maxChunkValue = new Vector2Int(halfSizeOfWorld.x - 1, halfSizeOfWorld.y - 1);
    }

    void Update()
    {
        Vector2Int currentChunkCoordinate = WorldToChunkCoordinates(player.transform.position);

        if (currentChunkCoordinate == _previousChunkCoordinate)
            return;

        _previousChunkCoordinate = currentChunkCoordinate;

        Vector2Int minChunkToGenerate = new Vector2Int(currentChunkCoordinate.x - renderDistance, currentChunkCoordinate.y - renderDistance);
        Vector2Int maxChunkToGenerate = new Vector2Int(currentChunkCoordinate.x + renderDistance, currentChunkCoordinate.y + renderDistance);

        // Checks if there are any currently generated chunks that shouldn't be generated anymore and deletes them
        Chunk[] chunksToDelete = _renderedChunks.FindAll(a => a.chunkCoordinates.x < minChunkToGenerate.x ||
                                     a.chunkCoordinates.y < minChunkToGenerate.y ||
                                     a.chunkCoordinates.x > maxChunkToGenerate.x ||
                                     a.chunkCoordinates.y > maxChunkToGenerate.y).ToArray();

        foreach (Chunk chunk in chunksToDelete)
        {
            DegenerateChunk(chunk);
        }

        GenerateNeighbourChunks(currentChunkCoordinate, minChunkToGenerate, maxChunkToGenerate);
    }

    IEnumerator GenerateNeighbourChunks(Vector2Int chunkToGenerate, Vector2Int minChunkToGenerate, Vector2Int maxChunkToGenerate)
    {
        yield return null;
        GenerateChunk(chunkToGenerate);
        
        chunkToGenerate.x += 1;
        CheckX();
        chunkToGenerate.x -= 1;
        CheckX();
        
        chunkToGenerate.y += 1;
        CheckY();
        chunkToGenerate.y -= 1;
        CheckY();

        void CheckX()
        {
            if (chunkToGenerate.x > maxChunkToGenerate.x || chunkToGenerate.x < minChunkToGenerate.x)
                return;
            StartCoroutine(GenerateNeighbourChunks(chunkToGenerate, minChunkToGenerate, maxChunkToGenerate));
        }

        void CheckY()
        {
            if (chunkToGenerate.y > maxChunkToGenerate.y || chunkToGenerate.y < minChunkToGenerate.y)
                return;
            GenerateNeighbourChunks(chunkToGenerate, minChunkToGenerate, maxChunkToGenerate);
        }
    }

    void GenerateChunk(Vector2Int chunkToGenerate)
    {
        if (chunkToGenerate.x < _minChunkValue.x || chunkToGenerate.y < _minChunkValue.y ||
            chunkToGenerate.x > _maxChunkValue.x || chunkToGenerate.y > _maxChunkValue.y)
            return;
        
        if (_renderedChunks.FindIndex(a => a.chunkCoordinates == chunkToGenerate) != -1)
            return;

        Vector2 tempOffset = Mathfs.Remap(_minChunkValue, _maxChunkValue, Vector2.zero, 
            maxWorldSizeInChunks * _chunkDimensions, chunkToGenerate);

        Vector2Int offset = new Vector2Int {x = Convert.ToInt32(tempOffset.x), y = Convert.ToInt32(tempOffset.y)};

        float[,] noiseMap = PerlinNoise.Generate(_chunkDimensions, _chunkDimensions, scale, octaves, persistance, lacunarity, offset.x, offset.y);

        Chunk newChunk = new Chunk(chunkToGenerate, new GameObject());
        newChunk.chunkObjects.transform.parent = transform;
        
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                Instantiate(blockToSpawn, new Vector3(x, Convert.ToInt32(noiseMap[x, y] * terrainScale),y), Quaternion.identity, newChunk.chunkObjects.transform);
            }
        }
        
        _renderedChunks.Add(newChunk);
    }

    void DegenerateChunk(Chunk chunkToDegenerate)
    {
        if (chunkToDegenerate.chunkCoordinates.x < _minChunkValue.x || chunkToDegenerate.chunkCoordinates.y < _minChunkValue.y ||
            chunkToDegenerate.chunkCoordinates.x > _maxChunkValue.x || chunkToDegenerate.chunkCoordinates.y > _maxChunkValue.y)
            return;
        
        int index;
        if ((index = _renderedChunks.FindIndex(a => a.chunkCoordinates == chunkToDegenerate.chunkCoordinates)) == -1)
            return;
        
        Destroy(_renderedChunks[index].chunkObjects);
        _renderedChunks.RemoveAt(index);
    }

    public Vector2Int WorldToChunkCoordinates(Vector3 coords)
    {
        Vector2Int chunkCoordinate = new Vector2Int(Convert.ToInt32(coords.x / 16), Convert.ToInt32(coords.z / 16));
        if (coords.x < 0 && coords.x > -16)
            chunkCoordinate.x = -1;
        
        if (coords.y < 0 && coords.y > -16)
            chunkCoordinate.y = -1;
        
        return chunkCoordinate;
    }
}
