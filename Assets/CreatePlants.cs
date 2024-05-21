using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlants : MonoBehaviour
{
    [SerializeField] private Plant plantPrefab;

    private const int maxPlantsInArea = 10;

    private List<Plant> currentPlantsInRange = new();

    [SerializeField] private SphereCollider sphereCollider;

    [SerializeField] private float spawnTime;

    [SerializeField] private Renderer playerRenderer;

    private PlayerMovement parentPlayer;

    private Color playerColor;
    private int playerNumber;

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<PlayerMovement>();

        playerColor = parentPlayer.PlayerColor;
        playerNumber = parentPlayer.PlayerNumber;
        playerRenderer.material.color = playerColor;

        StartCoroutine(SpawnPlantsInRange());
        StartCoroutine(ValidateExistingPlants());

    }

    private IEnumerator SpawnPlantsInRange()
    {
        while (true)
        {
            if (currentPlantsInRange.Count < maxPlantsInArea && parentPlayer.IsGrounded())
            {
                float worldPosY = transform.position.y;

                var plant = Instantiate(plantPrefab, GetRandomPosition(), Quaternion.identity);

                plant.transform.position = new Vector3(plant.transform.position.x, worldPosY, plant.transform.position.z);

                //TEMPORARY
                plant.PlantOwner = playerNumber;

                plant.GetComponent<Renderer>().material.color = playerColor;
                currentPlantsInRange.Add(plant);
            }

            yield return new WaitForSeconds(spawnTime);
        }
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
                print("Plant removed" + plant); // This line should be plant.transform.position
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
            currentPlantsInRange.Add(plant);
            plant.SetColor(playerColor);
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
