using System;
using System.Collections;
using System.Collections.Generic;
using Freya;
using FutureGames.Lib;
using UnityEngine;
using Random = System.Random;

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
    public int seed;
    public double treeChance = 0.1f;
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject woodBlock;
    public GameObject leafBlock;
    public GameObject player;
    public Vector2Int maxWorldSizeInChunks;
    public float chunkGenerationDelay = 0.2f;
    public int renderDistance = 6;
    public float terrainScale = 1f;
    public float scale = 1f;
    public int octaves = 1;
    public float persistance = 1f;
    public float lacunarity = 1f;

    private const int ChunkDimensions = 16;
    private readonly List<Chunk> _renderedChunks = new List<Chunk>();
    private Vector2Int _minChunkValue;
    private Vector2Int _maxChunkValue;
    private Vector2Int _previousChunkCoordinate = new Vector2Int(int.MaxValue, int.MaxValue);
    private int _previousRenderDistance;
    private Vector2Int _minChunkToGenerate;
    private Vector2Int _maxChunkToGenerate;

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
        _previousRenderDistance = renderDistance;
    }

    void Update()
    {
        Vector2Int currentChunkCoordinate = WorldToChunkCoordinates(player.transform.position);

        if (currentChunkCoordinate == _previousChunkCoordinate && _previousRenderDistance == renderDistance)
            return;

        _previousChunkCoordinate = currentChunkCoordinate;

        _minChunkToGenerate = new Vector2Int(currentChunkCoordinate.x - renderDistance, currentChunkCoordinate.y - renderDistance);
        _maxChunkToGenerate = new Vector2Int(currentChunkCoordinate.x + renderDistance, currentChunkCoordinate.y + renderDistance);

        // Checks if there are any currently generated chunks that shouldn't be generated anymore and deletes them
        Chunk[] chunksToDelete = _renderedChunks.FindAll(a => a.chunkCoordinates.x < _minChunkToGenerate.x ||
                                                              a.chunkCoordinates.y < _minChunkToGenerate.y ||
                                                              a.chunkCoordinates.x > _maxChunkToGenerate.x ||
                                                              a.chunkCoordinates.y > _maxChunkToGenerate.y).ToArray();

        foreach (Chunk chunk in chunksToDelete)
        {
            DegenerateChunk(chunk);
        }

        StartCoroutine(GenerateNeighbourChunks(currentChunkCoordinate));
        StartCoroutine(GenerateNeighbourChunks(_minChunkToGenerate));
        StartCoroutine(GenerateNeighbourChunks(_maxChunkToGenerate));
    }

    IEnumerator GenerateNeighbourChunks(Vector2Int chunkToGenerate)
    {
        GenerateChunk(chunkToGenerate);
        yield return new WaitForSeconds(chunkGenerationDelay);
        
        chunkToGenerate.x += 1;
        Check();
        yield return new WaitForSeconds(chunkGenerationDelay);
        chunkToGenerate.x -= 2;
        Check();
        yield return new WaitForSeconds(chunkGenerationDelay);
        
        chunkToGenerate.y += 1;
        Check();
        yield return new WaitForSeconds(chunkGenerationDelay);
        chunkToGenerate.y -= 2;
        Check();
        yield return new WaitForSeconds(chunkGenerationDelay);

        void Check()
        {
            if (IsChunkOutsideWorldBounds() || IsChunkGeneratedAlready())
                return;
            StartCoroutine(GenerateNeighbourChunks(chunkToGenerate));
        }

        bool IsChunkOutsideWorldBounds() => chunkToGenerate.x > _maxChunkToGenerate.x || chunkToGenerate.x < _minChunkToGenerate.x ||
                                            chunkToGenerate.y > _maxChunkToGenerate.y || chunkToGenerate.y < _minChunkToGenerate.y;

        bool IsChunkGeneratedAlready() => _renderedChunks.FindIndex(a => a.chunkCoordinates == chunkToGenerate) != -1;
    }

    void GenerateChunk(Vector2Int chunkToGenerate)
    {
        if (chunkToGenerate.x < _minChunkValue.x || chunkToGenerate.y < _minChunkValue.y ||
            chunkToGenerate.x > _maxChunkValue.x || chunkToGenerate.y > _maxChunkValue.y)
            return;
        
        if (_renderedChunks.FindIndex(a => a.chunkCoordinates == chunkToGenerate) != -1)
            return;

        Vector2 tempOffset = Mathfs.Remap(_minChunkValue, _maxChunkValue, Vector2.zero, 
            maxWorldSizeInChunks * ChunkDimensions, chunkToGenerate);

        Vector2Int offset = new Vector2Int {x = Convert.ToInt32(tempOffset.x), y = Convert.ToInt32(tempOffset.y)};

        float[,] noiseMap = PerlinNoise.Generate(ChunkDimensions, ChunkDimensions, scale, octaves, persistance, lacunarity, offset.x, offset.y);

        Chunk newChunk = new Chunk(chunkToGenerate, new GameObject());
        newChunk.chunkObjects.transform.parent = transform;
        newChunk.chunkObjects.name = $"Chunk {chunkToGenerate.x}, {chunkToGenerate.y}";
        
        int chunkSeed = seed * 100 + chunkToGenerate.x * 10 + chunkToGenerate.y;
        Random treesRandomGenerator = new Random(chunkSeed);
        
        UnityEngine.Random.State oldState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(chunkSeed);
        
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                Instantiate(grassBlock, new Vector3Int(x + chunkToGenerate.x * ChunkDimensions, 
                    Convert.ToInt32(noiseMap[x, y] * terrainScale),
                    y + chunkToGenerate.y * ChunkDimensions), Quaternion.identity, newChunk.chunkObjects.transform);
                
                Instantiate(dirtBlock, new Vector3Int(x + chunkToGenerate.x * ChunkDimensions, 
                    Convert.ToInt32(noiseMap[x, y] * terrainScale) - 1,
                    y + chunkToGenerate.y * ChunkDimensions), Quaternion.identity, newChunk.chunkObjects.transform);

                if (treesRandomGenerator.NextDouble() < treeChance)
                {
                    GenerateTree(new Vector3Int(x + chunkToGenerate.x * ChunkDimensions, 
                        Convert.ToInt32(noiseMap[x, y] * terrainScale + 1), 
                        y + chunkToGenerate.y * ChunkDimensions), newChunk.chunkObjects);
                }
            }
        }

        UnityEngine.Random.state = oldState;

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

    public void GenerateTree(Vector3Int treePosition, GameObject chunkObjects)
    {
        int amountOfLogs = UnityEngine.Random.Range(4, 6);

        for (int i = 0; i < amountOfLogs; i++)
        {
            Instantiate(woodBlock, treePosition, Quaternion.identity, chunkObjects.transform);
            treePosition.y++;
        }

        Vector3Int[] leafTopOfTree = new Vector3Int[]
        {
            // 9 blocks at the top
            treePosition, 
            new Vector3Int(treePosition.x + 1, treePosition.y, treePosition.z), 
            new Vector3Int(treePosition.x + 1, treePosition.y - 1, treePosition.z), 
            new Vector3Int(treePosition.x - 1, treePosition.y, treePosition.z), 
            new Vector3Int(treePosition.x - 1, treePosition.y - 1, treePosition.z), 
            new Vector3Int(treePosition.x, treePosition.y, treePosition.z + 1), 
            new Vector3Int(treePosition.x, treePosition.y - 1, treePosition.z + 1), 
            new Vector3Int(treePosition.x, treePosition.y, treePosition.z - 1), 
            new Vector3Int(treePosition.x, treePosition.y - 1, treePosition.z - 1), 
        };

        foreach (Vector3Int leaf in leafTopOfTree)
        {
            Instantiate(leafBlock, leaf, Quaternion.identity, chunkObjects.transform);
        }
    }

    public static Vector2Int WorldToChunkCoordinates(Vector3 coords)
    {
        Vector2Int chunkCoordinate = new Vector2Int(Convert.ToInt32(coords.x / ChunkDimensions), Convert.ToInt32(coords.z / ChunkDimensions));
        if (coords.x < 0 && coords.x > -16)
            chunkCoordinate.x = -1;
        
        if (coords.y < 0 && coords.y > -16)
            chunkCoordinate.y = -1;
        
        return chunkCoordinate;
    }
}