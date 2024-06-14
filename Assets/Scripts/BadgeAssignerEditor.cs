using Palmmedia.ReportGenerator.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BadgeAssigner))]
public class BadgeAssignerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BadgeAssigner badgeAssigner = (BadgeAssigner)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Badges"))
        {
            badgeAssigner.BadgePrefabs.Clear();
            badgeAssigner.BadgeTypes.Clear();
        }

        // Create a "dictionary" using the BadgeTypes and BadgePrefabs lists
        for (int i = 0; i < badgeAssigner.BadgePrefabs.Count; i++)
        {
            if (badgeAssigner.BadgeTypes.Count <= i)
            {
                continue; // Skip the rest of the loop iteration to prevent accessing null elements
            }

            EditorGUILayout.BeginHorizontal();

            GUILayoutOption[] buttonOptions = { GUILayout.Width(20) };

            if (GUILayout.Button("X", buttonOptions))
            {
                badgeAssigner.BadgePrefabs.RemoveAt(i);
                badgeAssigner.BadgeTypes.RemoveAt(i);
                continue; // Skip the rest of the loop iteration to prevent accessing null elements
            }

            try
            {

                badgeAssigner.BadgePrefabs[i] = EditorGUILayout.ObjectField(badgeAssigner.BadgePrefabs[i], typeof(GameObject), true) as GameObject;
                badgeAssigner.BadgeTypes[i] = (BadgeType)EditorGUILayout.EnumPopup(badgeAssigner.BadgeTypes[i]);
            }
            catch (System.ArgumentOutOfRangeException)
            {

            }


            EditorGUILayout.EndHorizontal();

        }

        // Button to add a new item
        if (GUILayout.Button("Add Item"))
        {
            // Add a new empty item to the shop data
            badgeAssigner.BadgePrefabs.Add(null);
            badgeAssigner.BadgeTypes.Add((BadgeType.MostDeaths));
        }
    }
}



