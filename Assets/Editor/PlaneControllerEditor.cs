using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaneController))]
public class PlaneControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        PlaneController planeController = (PlaneController)target;
        DrawDefaultInspector();
        if (GUILayout.Button("AlignHMD"))
        {
            planeController.alignPlaneWithHMD();
        }

    }
}
