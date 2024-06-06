using System.Collections;
using UnityEngine;
using static TeamColors.ColorEnum;
using GaiaElements;
using Players;
using TeamColors;

public class CreatePlants : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private Renderer playerRenderer;

    private Player parentPlayer;

    [SerializeField] private float heightOffset;

    private TEAMCOLOR teamColor;

    private bool canSpawnPlant = true;
    public float GrowthRate;

    // Animal Growth Proximity (Green)
    public bool AnimalProximityGrowth;
    public float MaxAnimalDistance;
    public float DistanceMultiplier;
    public float MinAnimalProximityGrowthRate;

    // Removal Trail (Red)
    public bool HasRemovalTrail;

    private AnimalLocator animalLocator;

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<Player>();
        parentPlayer.OnPlayerStart += BeginPlant;       
    }

    private void BeginPlant()
    {
        animalLocator = AnimalLocator.Instance;

        teamColor = parentPlayer.TeamColor;

        parentPlayer.OnPlayerNumberChange += () => teamColor = parentPlayer.TeamColor;

        playerRenderer.material.color = GetColor(teamColor);
        StartCoroutine(ReloadPlantSpread());

        print("create plants");
    }

    private float GetGrowthRate()
    {
        if (!AnimalProximityGrowth)
        {
            return GrowthRate;
        }

        var closestAnimal = animalLocator.GetClosestTransformOfTeam(transform.position, teamColor);

        if (closestAnimal == null)
        {
            return 0.45f;
        }

        var distance = Vector3.Distance(transform.position, closestAnimal.position);

        if (DistanceMultiplier <= 0)
        {
            DistanceMultiplier = 0.01f;
        }

        var variable = Mathf.Clamp(DistanceMultiplier * (MaxAnimalDistance * (distance / MaxAnimalDistance)), GrowthRate, MinAnimalProximityGrowthRate);

        print($"current var: {variable}");

        return variable;
    }

    /// <summary>
    /// Waits before the player can spawn a plant again
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadPlantSpread()
    {
        while (true)
        {
            if (!canSpawnPlant)
            {
                yield return new WaitForSeconds(GetGrowthRate());
                canSpawnPlant = true;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {      
        // Red player has no spread
        if (teamColor == TEAMCOLOR.RED)
        {
            return;
        }

        // Only spread influence if the player has "reloaded"
        if (!canSpawnPlant)
        {
            Debug.LogWarning("plant cannot be spawned");
            return;
        }

        if (!other.TryGetComponent<Plant>(out var plant))
        {
            return;
        }

        // Red Behaviour - Un-plants plants
        if (HasRemovalTrail)
        {
            // Do not unplant the player's own plants or unplanted plants
            if (plant.TeamColor != TEAMCOLOR.RED)
            {
                plant.UnPlant();
            }
        }
        else
        {
            print("plant spread");
            plant.PlayerParentTransform = parentPlayer.transform;
            plant.Activate(teamColor);
            plant.PlantSpreadCreep = parentPlayer.PlantSpreadCreep;
            canSpawnPlant = false;
        }
    }
}
