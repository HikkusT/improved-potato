using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Chunk {

    public Vector2i Position { get; private set; }

    private List<Vector3> vertices;
    private List<int> triangles;
    private int trianglesIndex;
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

    private float[,,] GenerateDensityMap(float[,] heightmap)
    {
        float[,,] densityMap = new float[settings.Size, settings.Height, settings.Size];

        for (int x = 0; x < settings.Size; x++)
            for (int y = 0; y < settings.Height; y++)
                for (int z = 0; z < settings.Size; z++)
                    if (y <= heightmap[x, z])
                        densityMap[x, y, z] = 1;
                    else
                        densityMap[x, y, z] = 0;

        return densityMap;
    }

    private void BuildTerrain(float[,,] densitymap)
    {
        int chunkIdentifier = Position.GetHashCode();
        chunk = new GameObject("Chunk" + chunkIdentifier.ToString());
        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
        chunk.transform.position = new Vector3(Position.X * settings.Size, 0, Position.Z * settings.Size);



        for (int x = 0; x < settings.Size; x++)
            for (int y = 0; y < settings.Height; y++)
                for (int z = 0; z < settings.Size; z++)
                    if (densitymap[x, y, z] > 0)
                        MakeCube(new Vector3(x, y, z), densitymap);
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

    private void MakeCube (Vector3 position, float[,,] densitymap)
    {
        for (int i = 0; i < 6; i++)
            if (ShouldDraw(position, (Directions) i, densitymap))
                MakeFace((Directions)i, position);
    }

    private void MakeFace(Directions dir, Vector3 position)
    {
        vertices.AddRange(VoxelMeshData.FaceVertices(dir, position));

        triangles.Add(trianglesIndex);
        triangles.Add(trianglesIndex + 1);
        triangles.Add(trianglesIndex + 2);
        triangles.Add(trianglesIndex);
        triangles.Add(trianglesIndex + 2);
        triangles.Add(trianglesIndex + 3);

        trianglesIndex += 4;
    }

    private bool ShouldDraw (Vector3 position, Directions dir, float[,,] densitymap)
    {
        Vector3 offsetToCheck = VoxelMeshData.NeighborOffsets[(int)dir];
        Vector3 neighborCoord = position + offsetToCheck;

        if ((int)neighborCoord.x < 0 || (int)neighborCoord.x >= settings.Size || (int)neighborCoord.y < 0 || (int)neighborCoord.y >= settings.Height || (int)neighborCoord.z < 0 || (int)neighborCoord.z >= settings.Size)
            return true;
        else if (densitymap[(int)neighborCoord.x, (int)neighborCoord.y, (int)neighborCoord.z] > 0)
            return false;
        else
            return true;
    }



    private void RequestTerrainData(Action<float[,,]> callback)
    {
        ThreadStart threadStart = delegate
        {
            TerrainDataCalculateThread(callback);
        };

        new Thread (threadStart).Start();
    }

    private void TerrainDataCalculateThread (Action<float[,,]> callback)
    {
        float[,] heightmap = GenerateHeightMap();
        float[,,] densitymap = GenerateDensityMap(heightmap);

        TerrainThreadInfo threadInfo = new TerrainThreadInfo(callback, densitymap);

        lock (ChunkManager.Instance.terrainThreadInfoQueue)
        {
            ChunkManager.Instance.terrainThreadInfoQueue.Enqueue(threadInfo);
        }
    }

    private void OnTerrainDataReceived (float[,,] densitymap)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        trianglesIndex = 0;

        BuildTerrain(densitymap);

        UpdateMesh();
    }

    void UpdateMesh ()
    {
        Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        chunk.GetComponent<MeshRenderer>().material = Resources.Load("TemporaryTestMat", typeof(Material)) as Material;
    }
}

public static class VoxelMeshData
{
    private static Vector3[] vertices =
    {
        new Vector3 (0.5f, 0.5f, 0.5f),
        new Vector3 (-0.5f, 0.5f, 0.5f),
        new Vector3 (-0.5f, -0.5f, 0.5f),
        new Vector3 (0.5f, -0.5f, 0.5f),
        new Vector3 (-0.5f, 0.5f, -0.5f),
        new Vector3 (0.5f, 0.5f, -0.5f),
        new Vector3 (0.5f, -0.5f, -0.5f),
        new Vector3 (-0.5f, -0.5f, -0.5f)
    };

    private static int[][] faces =
    {
        new int[] {0, 1, 2, 3},
        new int[] {5, 0, 3, 6},
        new int[] {4, 5, 6, 7},
        new int[] {1, 4, 7, 2},
        new int[] {5, 4, 1, 0},
        new int[] {3, 2, 7, 6}
    };

    public static Vector3[] NeighborOffsets =
    {
        new Vector3 (0, 0, 1f),
        new Vector3 (1f, 0, 0),
        new Vector3 (0, 0, -1f),
        new Vector3 (-1f, 0, 0),
        new Vector3 (0, 1f, 0),
        new Vector3 (0, -1f, 0)
    };

    public static Vector3[] FaceVertices (Directions dir, Vector3 initialPos)
    {
        Vector3[] fv = new Vector3[4];
        for (int i = 0; i < 4; i++)
            fv[i] = (vertices[faces[(int)dir][i]] + initialPos);

        return fv;
    }
}
