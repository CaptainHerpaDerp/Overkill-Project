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

    [Header("Paint Properties")]
    [SerializeField] private Vector3 plantSpawnOffset;
    [SerializeField] private int layerMask;
    [SerializeField] private float raycastDistance;

    private AnimalLocator animalLocator;

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<Player>();
        animalLocator = AnimalLocator.Instance;

        teamColor = parentPlayer.TeamColor;

        parentPlayer.OnPlayerNumberChange += () => teamColor = parentPlayer.TeamColor;

        playerRenderer.material.color = GetColor(teamColor);

        // Red player has no reload
        if (teamColor != TEAMCOLOR.RED)
        StartCoroutine(ReloadPlantSpread());
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
        // Only spread influence if the player has "reloaded"
        if (!canSpawnPlant)
        {
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
            plant.PlayerParentTransform = parentPlayer.transform;
            plant.Activate(teamColor);
            plant.PlantSpreadCreep = parentPlayer.PlantSpreadCreep;
            canSpawnPlant = false;
        }
    }

    private Vector3 GetRandomPosition()
    {
        float radius = sphereCollider.radius;

        var randomX = Random.Range(-radius, radius);
        var randomZ = Random.Range(-radius, radius);

        return transform.position + new Vector3(randomX, 0, randomZ);
    }
}
