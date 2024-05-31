using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPreferences", menuName = "Player/PlayerPreferences")]
public class PlayerPreferences : ScriptableObject
{
    public float GrowthRate;

    public bool PlantSpreadCreep;

    public bool AnimalProximityGrowth;
    public float MinAnimalProximityGrowthRate;
    public float MaxDistanceToAnimal;
    public float DistanceMultiplier;

    public bool HasRemovalTrail;
}
