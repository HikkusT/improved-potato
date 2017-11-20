using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Chunk {

    public Vector2i Position { get; private set; }

    private ChunkSettings settings;
    private GameObject chunk = null;



    public Chunk (int _x, int _z, ChunkSettings _settings)
    {
        Position = new Vector2i();
        Position.X = _x;
        Position.Z = _z;
        settings = _settings;
    }

    public Chunk(Vector2i _pos, ChunkSettings _settings)
    {
        Position = _pos;
        settings = _settings;
    }



    public void CreateTerrain()
    {
        RequestTerrainData(OnTerrainDataReceived);
    }

    public void DestroyTerrain()
    {
        //Debug.Log("Destroying " + chunk.name);
        GameObject.Destroy(chunk);
    }

    private float[,] GenerateHeightMap()
    {
        float[,] heights = NoiseGenerator.Perlin2D(Position.ToVector2(), settings.OffSet, settings.Size, settings.Scale, settings.Layers, settings.Persistance, settings.Lacunarity);

        for (int x = 0; x < settings.Size; x ++)
            for (int z = 0; z < settings.Size; z ++)
                heights[x, z] = (int)Mathf.Round(heights[x, z] * settings.Height);

        return heights;
    }

    private void BuildTerrain(float[,] heightmap)
    {
        int chunkIdentifier = Position.GetHashCode();
        chunk = new GameObject("Chunk" + chunkIdentifier.ToString());
        chunk.transform.position = new Vector3(0, 0, 0);

        for (int x = 0; x < settings.Size; x++)
            for (int z = 0; z < settings.Size; z++)
            {
                int xCoord = Position.X * settings.Size + x;
                int zCoord = Position.Z * settings.Size + z;

                GameObject voxel = GameObject.Instantiate(LevelManager.Instance.voxel, new Vector3(xCoord, heightmap[x, z], zCoord), Quaternion.identity, chunk.transform);
                voxel.isStatic = true;
            }
    }

    private void Combine()
    {
        StaticBatchingUtility.Combine(chunk);

        //chunk.AddComponent<MeshFilter>();
        //chunk.AddComponent<MeshRenderer>();
        //Material _material;

        //MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        //CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        //for (int i = 0; i < combine.Length; i++)
        //{
        //    combine[i].mesh = meshFilters[i].sharedMesh;
        //    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //    meshFilters[i].gameObject.active = false;
        //}

        //chunk.GetComponent<MeshFilter>().mesh = new Mesh();
        //chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //chunk.active = true;
    }

    private void RequestTerrainData(Action<float[,]> callback)
    {
        ThreadStart threadStart = delegate
        {
            TerrainDataCalculateThread(callback);
        };

        new Thread (threadStart).Start();
    }

    private void TerrainDataCalculateThread (Action<float[,]> callback)
    {
        float[,] heightmap = GenerateHeightMap();
        TerrainThreadInfo threadInfo = new TerrainThreadInfo(callback, heightmap);

        lock (ChunkManager.Instance.terrainThreadInfoQueue)
        {
            ChunkManager.Instance.terrainThreadInfoQueue.Enqueue(threadInfo);
        }
    }

    private void OnTerrainDataReceived (float[,] heightmap)
    {
        BuildTerrain(heightmap);

        Combine();
    }
}
