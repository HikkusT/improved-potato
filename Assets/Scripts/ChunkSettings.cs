using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSettings {

	public int Size { get; private set; }

    public int Height { get; private set; }

    public float Scale { get; private set; }

    public int Layers { get; private set; }

    public float Persistance { get; private set; }

    public float Lacunarity { get; private set; }

    public Vector2i[] OffSet { get; private set; }

    public ChunkSettings(int _size, int _height, float _scale, int _layers, float _persistance, float _lacunarity)
    {
        Size = _size;
        Height = _height;
        Scale = _scale;
        Layers = _layers;
        Persistance = _persistance;
        Lacunarity = _lacunarity;

        System.Random prng = new System.Random(LevelManager.Instance.Seed);
        OffSet = new Vector2i[_layers];
        for (int i = 0; i < _layers; i++)
            OffSet[i] = new Vector2i(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
    }

}
