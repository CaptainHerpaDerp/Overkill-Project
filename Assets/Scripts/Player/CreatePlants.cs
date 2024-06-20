using System.Collections;
using System.Collections.Generic;
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
    public float GrowthRate;
    [SerializeField] private float plantRetrieveRate;

    // Animal Growth Proximity (Green)
    public bool AnimalProximityGrowth;
    public float MaxAnimalDistance;
    public float DistanceMultiplier;
    public float MinAnimalProximityGrowthRate;

    // Removal Trail (Red)
    public bool HasRemovalTrail;

    private AnimalLocator animalLocator;
    [SerializeField] private LayerMask plantLayer;
    private List<Plant> surroundingPlants = new List<Plant>();
    private HashSet<Plant> plantSet = new HashSet<Plant>();

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<Player>();
        parentPlayer.OnPlayerStart += BeginPlant;

        StartCoroutine(UpdatePlants());
        StartCoroutine(ConvertSurroundingPlants());
    }

    private void BeginPlant()
    {
        animalLocator = AnimalLocator.Instance;
        teamColor = parentPlayer.TeamColor;
        parentPlayer.OnPlayerNumberChange += () => teamColor = parentPlayer.TeamColor;
        HasRemovalTrail = parentPlayer.HasRemovalTrail;

        playerRenderer.material.color = GetColor(teamColor);
        //print("create plants");
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

        float finalRate = Mathf.Clamp(DistanceMultiplier * (distance / MaxAnimalDistance), MinAnimalProximityGrowthRate, 1);
        //print("Final Rate: " + finalRate);
        return finalRate;
    }

    private IEnumerator UpdatePlants()
    {
        while (true)
        {
            UpdateSurroundingPlants();
            yield return new WaitForSeconds(plantRetrieveRate);
        }
    }

    private void UpdateSurroundingPlants()
    {
        var colliders = Physics.OverlapSphere(transform.position, sphereCollider.radius, plantLayer);
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out Plant plant)) continue;

            if (!plantSet.Contains(plant))
            {
                plantSet.Add(plant);
                surroundingPlants.Add(plant);
            }
        }
    }

    private IEnumerator ConvertSurroundingPlants()
    {
        while (true)
        {
            List<Plant> plantsToRemove = new List<Plant>();

            for (int i = 0; i < surroundingPlants.Count; i++)
            {
                Plant plant = surroundingPlants[i];

                if (HasRemovalTrail)
                {
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
                }

                plantsToRemove.Add(plant);

                yield return new WaitForSeconds(GetGrowthRate());
            }

            // Now remove the plants that were marked for removal
            foreach (var plant in plantsToRemove)
            {
                surroundingPlants.Remove(plant);
                // If you have another collection like plantSet, make sure to remove from that as well
                plantSet.Remove(plant); // Uncomment if plantSet is used
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Plant plant)) return;

        if (plantSet.Contains(plant))
        {
            plantSet.Remove(plant);
            surroundingPlants.Remove(plant);
        }
    }
}
