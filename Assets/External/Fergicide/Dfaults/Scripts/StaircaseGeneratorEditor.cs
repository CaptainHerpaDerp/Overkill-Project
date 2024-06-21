using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

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

#endif