using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject Player;

    public int RenderDistance = 3;
    public static Vector3 PlayerPosition = new Vector3(0, 0, 0); 

    private int chunkSize;
    private int chunkHeight;
    private Vector2i prevPos;
    private Vector2i currentPos;

    private ChunkManager chunkManager;

	void Start () {

	}
	
	void Update () {
        PlayerPosition = Player.transform.position;
	}
}
