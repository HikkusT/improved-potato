using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

public class ChunkManager : MonoBehaviour{

    public Queue<TerrainThreadInfo> terrainThreadInfoQueue = new Queue<TerrainThreadInfo>();

    private ChunkSettings settings;
    private Dictionary<Vector2i, Chunk> loadedChunks;

    private int renderDistance;
    private Vector2i prevPos;
    private Vector2i currentPos;

    public static ChunkManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        settings = new ChunkSettings(LevelManager.Instance.ChunkSize, LevelManager.Instance.ChunkHeight, LevelManager.Instance.Scale
                                     , LevelManager.Instance.Layers, LevelManager.Instance.RateOfAmplitude, LevelManager.Instance.RateOfFrequency);

        loadedChunks = new Dictionary<Vector2i, Chunk>();


        renderDistance = GetComponent<GameManager>().RenderDistance;
        prevPos = new Vector2i(100000000, 100000000);
    }

    private void Update()
    {
        if (terrainThreadInfoQueue.Count > 0)
            for (int i = 0; i < terrainThreadInfoQueue.Count; i++)
            {
                TerrainThreadInfo threadInfo = terrainThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.calculatedInfo);
            }

        currentPos = GetChunkAtPosition(GameManager.PlayerPosition);
        if (!currentPos.Equals(prevPos))
        {
            prevPos = currentPos;
            UpdateTerrain(currentPos, renderDistance);
        }
    }



    public void UpdateTerrain(Vector2i centerChunk, int radius)
    {
        List<Vector2i> currentChunks = GetChunksInRange(centerChunk, radius);
        List<Vector2i> loadedChunksPos = loadedChunks.Keys.ToList();
        List<Vector2i> chunksToDestroy = loadedChunksPos.Except(currentChunks).ToList();
        List<Vector2i> chunksToCreate = currentChunks.Except(loadedChunksPos).ToList();

        foreach (Vector2i chunk in chunksToCreate)
            CreateChunk(chunk);

        foreach (Vector2i chunk in chunksToDestroy)
            DestroyChunk(chunk);
    }

    public Vector2i GetChunkAtPosition (Vector3 position)
    {
        int x = (int)Mathf.Floor(position.x / settings.Size);
        int z = (int)Mathf.Floor(position.z / settings.Size);

        return new Vector2i(x, z);
    }

    private List<Vector2i> GetChunksInRange (Vector2i currentChunk, int radius)
    {
        List<Vector2i> chunks = new List<Vector2i>();

        for (int i = -radius; i <= radius; i++)
            for (int j = -radius; j <= radius; j++)
                    chunks.Add(new Vector2i(currentChunk.X + i, currentChunk.Z + j));

        return chunks;
    }

    private void CreateChunk(Vector2i chunkPos)
    {
        Chunk chunk = new Chunk(chunkPos.X, chunkPos.Z, settings);
        chunk.CreateTerrain();
        loadedChunks.Add(chunkPos, chunk);
    }

    private void DestroyChunk(Vector2i chunkPos)
    {
        Chunk chunk = loadedChunks[chunkPos];
        chunk.DestroyTerrain();
        loadedChunks.Remove(chunkPos);
    }

}

public struct TerrainThreadInfo
{
    public readonly Action<float[,,]> callback;
    public readonly float[,,] calculatedInfo;

    public TerrainThreadInfo(Action<float[,,]> callback, float[,,] parameter)
    {
        this.callback = callback;
        this.calculatedInfo = parameter;
    }

}
