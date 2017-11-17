using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public DrawMode drawMode;

    public int Seed = 0;
    public Vector2 Start = new Vector2(0, 0);
    public int MapSize = 100;
    public float Scale = 1;
    public int Layers = 3;
    [Range(0,1)]
    public float RateOfAmplitude = 0.5f;
    public float RateOfFrequency = 2f;
    public bool AutoUpdate;
    public Renderer textureRender;

    public MinimapTerrainType[] regions;

    public void GenerateMap ()
    {
        //Vector3 playerPosition = gameObject.GetComponent<GameManager>().Player.transform.position;
        //Vector2i startPosition = new Vector2(playerPosition.x - (MapSize / 2), playerPosition.z - (MapSize / 2)).ToVector2i();


        System.Random prng = new System.Random(Seed);
        Vector2i[] offSet = new Vector2i[Layers];
        for (int i = 0; i < Layers; i++)
            offSet[i] = new Vector2i(prng.Next(-10000, 10000), prng.Next(-10000, 10000));

        float[,] heightMap = NoiseGenerator.Perlin2D(Start, offSet, MapSize, Scale, Layers, RateOfAmplitude, RateOfFrequency);

        Color[] colorMap = new Color[MapSize * MapSize];
        for (int x = 0; x < MapSize; x ++)
            for (int z = 0; z < MapSize; z ++)
            {
                float currentHeight = heightMap[x, z];

                for (int i = 0; i < regions.Length; i ++)
                    if (currentHeight <= regions[i].Height)
                    {
                        colorMap[x * MapSize + z] = regions[i].Color;
                        break;
                    }
            }

        if (drawMode == DrawMode.HeightMap)
            DrawNoiseTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        else if (drawMode == DrawMode.ColorMap)
            DrawNoiseTexture(TextureGenerator.TextureFromColorMap(colorMap, MapSize));
    }

    private void DrawNoiseTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(MapSize, 1, MapSize);
    }

}

[System.Serializable]
public struct MinimapTerrainType
{
    public string Name;
    public float Height;
    public Color Color;
}