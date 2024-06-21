using GaiaElements;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SurroundingPlant : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;

    public ColorEnum.TEAMCOLOR TeamColour;

    [SerializeField] private bool excludeNeutralPlants;
    [SerializeField] private LayerMask plantLayer;

    [Header("If the resulting list of plants is empty, expand the sphere collider and retry the method until a plant is found")]
    [SerializeField] private bool expandOnEmptyList;
    [SerializeField] private float incrementExpandAmount;
    [SerializeField] private float maxExpandAmount;
    private float initialSphereRadius;

    [SerializeField] private float overlapRadius;

    private void Start()
    {
        if (sphereCollider != null)
        {
            sphereCollider = GetComponent<SphereCollider>();
            initialSphereRadius = sphereCollider.radius;
            sphereCollider.isTrigger = true;
        }
    }

    public void SetColliderRadius(float radius)
    {
        sphereCollider.radius = radius;
    }

    public List<Plant> GetSurroundingOpponentPlantsList(Transform atTransform = null, bool includeNeutral = false)
    {
        float captureRadius = 0;
        if (sphereCollider != null)
        {
            captureRadius = sphereCollider.radius;
        }
        else
        {
            captureRadius = overlapRadius;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, captureRadius, layerMask: plantLayer);
        List<Plant> opponentPlants = new();

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Plant plant);

            if (plant == null)
                continue;

            // Check if the plant is owned by the player
            if (includeNeutral && plant.TeamColor != TeamColour)
            {
                opponentPlants.Add(plant);
            }

            if (!includeNeutral && plant.TeamColor != TeamColour && plant.TeamColor != ColorEnum.TEAMCOLOR.DEFAULT)
            {
                opponentPlants.Add(plant);
            }
        }

        // If the list of opponent plants is 0, expand the sphere collider and try again
        if (expandOnEmptyList && opponentPlants.Count == 0)
        {
            bool foundPlant = false;
            while (!foundPlant && sphereCollider.radius < maxExpandAmount)
            {
                sphereCollider.radius += incrementExpandAmount;

                Vector3 overlapPos;
                if (atTransform != null)
                    overlapPos = atTransform.position;
                else
                    overlapPos = transform.position;

                hitColliders = Physics.OverlapSphere(overlapPos, sphereCollider.radius, layerMask: plantLayer);

                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.TryGetComponent(out Plant plant);

                    if (plant == null)
                        continue;

                    // Check if the plant is owned by the player
                    if (plant.TeamColor != TeamColour && plant.TeamColor != ColorEnum.TEAMCOLOR.DEFAULT)
                    {
                        opponentPlants.Add(plant);
                        foundPlant = true;
                    }
                }
            }

            // Reset the sphere collider radius regardless of the result
            sphereCollider.radius = initialSphereRadius;
        }

        return opponentPlants;
    }

    public List<Plant> GetSurroundingNeutralPlantsList()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius, layerMask: plantLayer);
        List<Plant> opponentPlants = new();

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Plant plant);

            if (plant == null)
                continue;

            // Check if the plant is owned by the player
            if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                opponentPlants.Add(plant);
            }
        }

        return opponentPlants;
    }

    /// <summary>
    /// Return the amount of plants surrounding the player
    /// </summary>
    /// <returns></returns>
    public float GetSurroundingPlants()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius, layerMask: plantLayer);

        int totalPlants = 0;
        int playerPlants = 0;

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Plant plant);

            if (plant == null)
                continue;

            if (excludeNeutralPlants && plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
                continue;

            totalPlants++;

            // Check if the plant is owned by the player
            if (plant.TeamColor == TeamColour)
            {
                playerPlants++;
            }
        }

        // With a max value of 1, get the percentage of plants owned by the player
        if (totalPlants == 0)
        {
            // print("No plants found");
            return 0.01f;
        }

        return (float)playerPlants / (float)totalPlants;
    }

    public float GetSurroundingPlantsClamped()
    {
        return Mathf.Clamp(GetSurroundingPlants(), 0.01f, 1);
    }
}
