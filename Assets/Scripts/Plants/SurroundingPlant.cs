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

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.isTrigger = true;
    }

    public void SetColliderRadius(float radius)
    {
        sphereCollider.radius = radius;
    }

    public List<Plant> GetSurroundingOpponentPlantsList()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);

        List<Plant> opponentPlants = new();

        foreach (var hitCollider in hitColliders)
        {
            hitCollider.TryGetComponent(out Plant plant);

            if (plant == null)
                continue;
          
            // Check if the plant is owned by the player
            if (plant.TeamColor != TeamColour)
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);

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

        //Debug.Log("Player " + playerNumber + " has " + playerPlants + " plants out of " + totalPlants);

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
