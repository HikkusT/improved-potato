using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {

	public static float[,] Perlin2D(Vector2i start, int mapSize, int mapHeight, float scale, int layers, float persistance, float lacunarity)
    {
        float[,] heights = new float[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
            for (int z = 0; z < mapSize; z++)
            {
                float amplitude = 1f;
                float frequency = 1f;

                float height = 0;
                float maxHeight = 0;

                for (int i = 0; i < layers; i++)
                {
                    float xCoord = (start.X * mapSize + (float)x) / scale * frequency;
                    float zCoord = (start.Z * mapSize + (float)z) / scale * frequency;

                    height += ((Mathf.PerlinNoise(xCoord, zCoord))) * amplitude;
                    maxHeight += amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                heights[x, z] = height / maxHeight;

                heights[x, z] = (int)Mathf.Round(height * mapHeight);
            }

        return heights;
    }

    public static float[,] Perlin2D(Vector2i start, int mapSize, float scale, int layers, float persistance, float lacunarity)
    {
        float[,] heights = new float[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
            for (int z = 0; z < mapSize; z++)
            {
                float amplitude = 1f;
                float frequency = 1f;

                float height = 0;
                float maxHeight = 0;

                for (int i = 0; i < layers; i++)
                {
                    float xCoord = (start.X * mapSize + (float)x) / scale * frequency;
                    float zCoord = (start.Z * mapSize + (float)z) / scale * frequency;

                    height += ((Mathf.PerlinNoise(xCoord, zCoord))) * amplitude;
                    maxHeight += amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                heights[x, z] = height/maxHeight;
            }

        return heights;
    }

}
