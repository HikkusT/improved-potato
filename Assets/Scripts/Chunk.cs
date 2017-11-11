using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    public Vector2i Position { get; private set; }

    private ChunkSettings settings;

    //private NoiseGenerator noiseGenerator = new NoiseGenerator();

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
        float[,] heightmap = GenerateHeightMap();

        BuildTerrain(heightmap);

        Combine();
    }

    public void DestroyTerrain()
    {
        //Debug.Log("Destroying " + chunk.name);
        GameObject.Destroy(chunk);
    }

    private float[,] GenerateHeightMap()
    {
        float[,] heights = NoiseGenerator.Perlin2D(Position, settings.OffSet, settings.Size, settings.Height, settings.Scale, settings.Layers, settings.Persistance, settings.Lacunarity);

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
}
