using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator{

	public static Texture2D TextureFromColorMap (Color[] colorMap, int size)
    {
        Texture2D texture = new Texture2D(size, size);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap (float[,] heightMap)
    {
        int size = heightMap.GetLength(1);

        Color[] colorMap = new Color[size * size];
        for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
                colorMap[x * size + z] = Color.Lerp(Color.black, Color.white, heightMap[x, z]);

        return TextureFromColorMap(colorMap, size);
    }
}
