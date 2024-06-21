using GameManagement;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(GlobalPlantPaint))]
public class GlobalPlantPainterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GlobalPlantPaint painter = (GlobalPlantPaint)target;

        if (GUILayout.Button("Place Plants"))
        {
            painter.PlacePlants();
        }

        if (GUILayout.Button("Clear Plants"))
        {
            painter.ClearPlants();
        }

        if (GUILayout.Button("Verify Plants"))
        {
            painter.VerifyPlantPositions();
        }
    }
}
#endif