using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChunkManager{

    private ChunkSettings settings = new ChunkSettings(LevelManager.Instance.ChunkSize, LevelManager.Instance.ChunkHeight, LevelManager.Instance.Scale
                                                       , LevelManager.Instance.Layers, LevelManager.Instance.RateOfAmplitude, LevelManager.Instance.RateOfFrequency);

    private Dictionary<Vector2i, Chunk> loadedChunks = new Dictionary<Vector2i, Chunk>();



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
        //Debug.Log("Create");

        Chunk chunk = new Chunk(chunkPos.X, chunkPos.Z, settings);
        chunk.CreateTerrain();
        loadedChunks.Add(chunkPos, chunk);
    }

    private void DestroyChunk(Vector2i chunkPos)
    {
        //Debug.Log("Destroy");

        Chunk chunk = loadedChunks[chunkPos];
        chunk.DestroyTerrain();
        loadedChunks.Remove(chunkPos);
    }

}
