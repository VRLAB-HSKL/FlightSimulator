using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaneController))]
public class PlaneControllerEditor : Editor
{
    /// <summary>
    /// Adds a button to align the HMD with the rotation of the cockpit.
    /// </summary>
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
