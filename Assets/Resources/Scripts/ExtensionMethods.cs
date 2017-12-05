using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods{

	public static Vector2i ToVector2i (this Vector2 pos)
    {
        return new Vector2i((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y));
    }
}
