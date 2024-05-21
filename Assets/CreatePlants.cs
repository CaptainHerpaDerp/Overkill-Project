using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlants : MonoBehaviour
{
    [SerializeField] private Plant plantPrefab;

    private const int maxPlantsInArea = 7;

    [SerializeField] private List<Plant> currentPlantsInRange = new();

    [SerializeField] private SphereCollider sphereCollider;

    [SerializeField] private float spawnTime;

    [SerializeField] private Renderer playerRenderer;

    private PlayerMovement parentPlayer;

    [SerializeField] private float raycastDistance;

    [SerializeField] private float heightOffset;

    private Color playerColor;
    private int playerNumber;

    private void Start()
    {
        parentPlayer = transform.parent.GetComponent<PlayerMovement>();

        playerColor = parentPlayer.PlayerColor;
        playerNumber = parentPlayer.PlayerNumber;

        parentPlayer.OnPlayerColorChange += () => playerColor = parentPlayer.PlayerColor;
        parentPlayer.OnPlayerNumberChange += () => playerNumber = parentPlayer.PlayerNumber;

        parentPlayer.OnPlayerRespawn += () =>
        {
            StopAllCoroutines();
            StartCoroutine(SpawnPlantsInRange());
        };

        playerRenderer.material.color = playerColor;

        StartCoroutine(SpawnPlantsInRange());
        StartCoroutine(ValidateExistingPlants());
    }

    private IEnumerator SpawnPlantsInRange()
    {
        // Do not spawn plants immediately
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (currentPlantsInRange.Count < maxPlantsInArea)
            {
                Vector3 worldPos = GetGroundPosition(transform.position);

                // If worldpos returns as zero, it means that the player is not grounded
                if (worldPos == Vector3.zero)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                float worldPosY = worldPos.y + heightOffset;

                var plant = Instantiate(plantPrefab, GetRandomPosition(), Quaternion.identity);

                // Register the plant in the ScoreManager
                ScoreManager.Instance.RegisterPlant(plant.PlantID, playerNumber);

                plant.transform.position = new Vector3(plant.transform.position.x, worldPosY, plant.transform.position.z);

                //TEMPORARY
                plant.PlantOwner = playerNumber;      

                plant.SetColor(playerColor);

                currentPlantsInRange.Add(plant);
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, raycastDistance, layerMask: 6))
        {
            // Rather than get the ground position, get the hit point

            return hit.point;
        }
        else
        {
            Debug.LogWarning("No ground found");
            return Vector3.zero;
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
        if (!parentPlayer.IsGrounded())
            return;

        if (!other.TryGetComponent<Plant>(out var plant))
        {
            return;
        }

        if (!currentPlantsInRange.Contains(plant))
        {
            print("added plant");
            currentPlantsInRange.Add(plant);

            plant.PlantOwner = playerNumber;
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
