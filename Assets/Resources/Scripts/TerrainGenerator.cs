using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public int chunkWidth = 32;
    public int maxHeight = 20;
    public float scale = 0.1f;
    public GameObject voxel;

    private int mapSize2D;
    private int[,] voxels2D;
    private int[] heightmap;

    private void Start()
    {
        mapSize2D = chunkWidth * chunkWidth;

        Initialize2DVoxels();
        heightmap = Generate2dPerlinNoise();

        CreateTerrain();
    }

    private void Initialize2DVoxels()
    {
        voxels2D = new int[mapSize2D, 2];

        int addIndex = 0;
        for (int x = 0; x < chunkWidth; x++)
            for (int y = 0; y < chunkWidth; y++)
            {
                voxels2D[addIndex, 0] = x;
                voxels2D[addIndex, 1] = y;

                addIndex++;
            }
    }


    private int[] Generate2dPerlinNoise()
    {
        int[] heights = new int[mapSize2D];
        float normalizedHeight = 0;
        int height = 0;
        float randomPerlinStart = Random.Range(0f, 10f);

        int addIndex = 0;
        for (int x = 0; x < chunkWidth; x++)
            for (int y = 0; y < chunkWidth; y++)
            {
                normalizedHeight = CalculateHeight(x, y, randomPerlinStart);
                height = (int)Mathf.Round(normalizedHeight * maxHeight);
                heights[addIndex] = height;

                addIndex++;
            }

        return heights;
    }

    private float CalculateHeight(int x, int y, float start)
    {
        float xCoord = (start + (float)x / chunkWidth) * scale;
        float yCoord = (start + (float)y / chunkWidth) * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return sample;
    }

    private void CreateTerrain()
    {
        Vector3 position;

        for (int i = 0; i < mapSize2D; i++)
        {
            position = new Vector3(voxels2D[i, 0], heightmap[i], voxels2D[i, 1]);
            Instantiate(voxel, position, Quaternion.identity);
        }
    }

}

