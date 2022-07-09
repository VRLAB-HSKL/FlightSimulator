using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/**
 * Provides functionality to the MapGenerator Component in the Editor view
 */
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if (DrawDefaultInspector())// gets called when a value gets changed
        {
            if (mapGen.autoUpdate) // if autoupdate generate a new noise map on value change
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}
