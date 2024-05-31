using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TeamColors.ColorEnum;
using GaiaElements;
using Players;

public class CreatePlants : MonoBehaviour
{
    [SerializeField] private Plant plantPrefab;

    private const int maxPlantsInArea = 7;

    [SerializeField] private List<Plant> currentPlantsInRange = new();

    [SerializeField] private SphereCollider sphereCollider;

    [SerializeField] private float spawnTime;

    [SerializeField] private Renderer playerRenderer;

    private Player parentPlayer;

    [SerializeField] private float heightOffset;

    private TEAMCOLOR teamColor;

    [Header("Paint Properties")]
    [SerializeField] private Vector3 plantSpawnOffset;
    [SerializeField] private int layerMask;
    [SerializeField] private float raycastDistance;

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<Player>();

        teamColor = parentPlayer.TeamColor;

        parentPlayer.OnPlayerNumberChange += () => teamColor = parentPlayer.TeamColor;

        parentPlayer.OnPlayerRespawn += () =>
        {
            StopAllCoroutines();
            //StartCoroutine(SpawnPlantsInRange());
        };

        playerRenderer.material.color = GetColor(teamColor);

        StartCoroutine(ValidateExistingPlants());
    }

    private IEnumerator ValidateExistingPlants()
    {
        while (true)
        {
            ValidatePlants();
            yield return new WaitForSeconds(1);
        }
    }

    private void ValidatePlants()
    {
        List<Plant> plantsToRemove = new();

        // Check to see if the plants in the list are still in the sphere collider
        foreach (var plant in currentPlantsInRange)
        {
            if (plant == null)
            {
                plantsToRemove.Add(plant);
                continue;
            }

            if (!sphereCollider.bounds.Contains(plant.transform.position))
            {
               // print("Plant removed" + plant); // This line should be plant.transform.position
                plantsToRemove.Add(plant);
            }
        }

        foreach (var plantToRemove in plantsToRemove)
        {
            if (currentPlantsInRange.Contains(plantToRemove))
            currentPlantsInRange.Remove(plantToRemove);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Plant>(out var plant))
        {
            return;
        }

        if (!currentPlantsInRange.Contains(plant))
        {
           // print("added plant");
            currentPlantsInRange.Add(plant);

            plant.Activate(teamColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Plant>(out var plant))
        {
            return;
        }

        currentPlantsInRange.Remove(plant);
    }

    private Vector3 GetRandomPosition()
    {
        float radius = sphereCollider.radius;

        var randomX = Random.Range(-radius, radius);
        var randomZ = Random.Range(-radius, radius);

        return transform.position + new Vector3(randomX, 0, randomZ);
    }
}
