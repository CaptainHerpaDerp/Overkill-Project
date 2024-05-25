using GaiaElements;
using Players;
using TeamColors;
using UnityEngine;

public class SurroundingPlant : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;

    private ColorEnum.TEAMCOLOR playerNumber;

    private void Start()
    {
        Player parentPlayer = transform.parent.GetComponent<Player>();

        playerNumber = parentPlayer.TeamColor;
        parentPlayer.OnPlayerNumberChange += () => playerNumber = parentPlayer.TeamColor;
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

            totalPlants++;

            // Check if the plant is owned by the player
            if (plant.TeamColor == playerNumber)
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
}