using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StaircaseGenerator))]
public class StaircaseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StaircaseGenerator myScript = (StaircaseGenerator)target;
        if(GUILayout.Button("Generate Staircase"))
        {
            myScript.Generate();
        }

        DrawDefaultInspector();
    }
}
