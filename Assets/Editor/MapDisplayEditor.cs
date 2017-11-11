using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapDisplay))]
public class MapDisplayEditor : Editor {

    public override void OnInspectorGUI()
    {

        MapDisplay mapDisplay = (MapDisplay)target;

        if (DrawDefaultInspector())
            if (mapDisplay.AutoUpdate)
                mapDisplay.GenerateMap();

        if (GUILayout.Button("Generate"))
            mapDisplay.GenerateMap();
    }

}
