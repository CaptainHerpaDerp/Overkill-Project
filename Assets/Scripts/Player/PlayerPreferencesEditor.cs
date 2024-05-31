using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPreferences))]
public class PlayerPreferencesEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        PlayerPreferences playerPreferences = (PlayerPreferences)target;

        EditorGUILayout.LabelField("The player's default plant growth rate (plants/second)");
        playerPreferences.GrowthRate = EditorGUILayout.FloatField("Growth Rate", playerPreferences.GrowthRate);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("If the player's plants will grow automatically");
        playerPreferences.PlantSpreadCreep = EditorGUILayout.Toggle("Plant Spread Creep", playerPreferences.PlantSpreadCreep);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Player / Animal Growth Proximity - Player's grow rate depends on its proximity to an owned animal");
        playerPreferences.AnimalProximityGrowth = EditorGUILayout.Toggle("Animal Proximity Growth", playerPreferences.AnimalProximityGrowth);

        if (playerPreferences.AnimalProximityGrowth)
        {
            playerPreferences.MaxDistanceToAnimal = EditorGUILayout.FloatField("Max Distance To Animal", playerPreferences.MaxDistanceToAnimal);

            EditorGUILayout.LabelField("Player's growth rate at max distance to animal");
            playerPreferences.MinAnimalProximityGrowthRate = EditorGUILayout.FloatField("Animal Proximity Minimum Growth Rate", playerPreferences.MinAnimalProximityGrowthRate);

            EditorGUILayout.LabelField("The multiplier for the player's growth rate based on its distance to the animal");
            playerPreferences.DistanceMultiplier = EditorGUILayout.FloatField("Distance Multiplier", playerPreferences.DistanceMultiplier);
        }

        EditorGUILayout.Space(10);

        playerPreferences.HasRemovalTrail = EditorGUILayout.Toggle("Has Removal Trail", playerPreferences.HasRemovalTrail);

        EditorGUILayout.Space(10);

        // Set dirty to save the changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(playerPreferences);
        }
    }

}
