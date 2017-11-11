using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public int MapSize = 100;
    public float Scale = 1;
    public int Layers = 3;
    [Range(0,1)]
    public float RateOfAmplitude = 0.5f;
    public float RateOfFrequency = 2f;
    public bool AutoUpdate;

    public Renderer textureRender;


    public void GenerateMap ()
    {
        float[,] noiseMap = NoiseGenerator.Perlin2D(new Vector2i(0, 0), MapSize, Scale, Layers, RateOfAmplitude, RateOfFrequency);

        DrawNoiseMap(noiseMap);
    }

    private void DrawNoiseMap(float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(MapSize, MapSize);

        Color[] colorMap = new Color[MapSize * MapSize];
        for (int x = 0; x < MapSize; x++)
            for (int y = 0; y < MapSize; y++)
                colorMap[x * MapSize + y] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(MapSize, 1, MapSize);
    }

}
