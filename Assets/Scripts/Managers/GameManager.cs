using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int RenderDistance;
    public GameObject Player;

    private int chunkSize;
    private int chunkHeight;
    private Vector2i prevPos;
    private Vector2i currentPos;

    private ChunkManager chunkManager;

	void Start () {
        chunkManager = new ChunkManager();
        prevPos = new Vector2i(0, 0);
        chunkManager.UpdateTerrain(new Vector2i(0, 0), RenderDistance);
	}
	
	void Update () {
        currentPos = chunkManager.GetChunkAtPosition(Player.transform.position);
        if (!currentPos.Equals(prevPos))
        {
            prevPos = currentPos;
            chunkManager.UpdateTerrain(currentPos, RenderDistance);
        }
	}
}
