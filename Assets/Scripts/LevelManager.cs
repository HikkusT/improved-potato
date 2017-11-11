using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public GameObject voxel;
    public int ChunkSize = 16;
    public int ChunkHeight = 20;
    public float Scale;
    public int Layers = 3;
    public float RateOfAmplitude = 0.5f;
    public float RateOfFrequency = 2f;
    

    public static LevelManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
}
