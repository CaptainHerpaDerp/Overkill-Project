using GaiaElements;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

/// <summary>
/// Component attached to plant. Allows plant to automatically spread its influence to surrounding plants
/// </summary>
public class PlantCreepSpread : MonoBehaviour
{
    private Coroutine spreadCoroutine;

    // List to store any available plants 
    private List<Plant> availablePlants = new();

    [SerializeField] private float maxRadius;
    [SerializeField] private SurroundingPlant surroundingPlants;

    [Header("Every second, a roll is made from 1 to this number. If the roll is equal to 1, the plant will spread. This is the minimum roll chance, assuming the distance to the player is 0")]
    [SerializeField] private int rollChance;
    [SerializeField] private float maxDistanceModifier;
    [SerializeField] private float distanceMultiplier;

    Plant parentPlant;

    private Vector3 playerParentPosition => parentPlant.PlayerParentTransform.position;

    private ColorEnum.TEAMCOLOR teamColor => parentPlant.TeamColor;

    private void Start()
    {
        parentPlant = transform.parent.GetComponent<Plant>();

        if (parentPlant == null)
        {
            Debug.LogError("Problem in PlantCreepSpread: Parent plant could not be found in the parent transform!");
        }
        else
        {
            parentPlant.OnPlantSettingsChanged += ValidatePlantGrowthSettings;
        }

        // Initialize the surrounding plants script with the appropriate values
        surroundingPlants.SetColliderRadius(maxRadius);
        surroundingPlants.TeamColour = teamColor;
    }

    private void ValidatePlantGrowthSettings()
    {
        // Check if the plant parent has spreading enabled
        if (!parentPlant.PlantSpreadCreep)
        {
            if (spreadCoroutine != null)
            {
                print("stopping spread"); 

                StopCoroutine(spreadCoroutine);
                spreadCoroutine = null;

                gameObject.SetActive(false);
            }
        }
        else
        {
            print("starting spread");
            surroundingPlants.TeamColour = teamColor;
            spreadCoroutine ??= StartCoroutine(SpreadToSurroundingPlants());

            gameObject.SetActive(true);
        }
    }

    private IEnumerator SpreadToSurroundingPlants()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (Random.Range(1, Mathf.Clamp(rollChance * GetDistanceModifier(), rollChance, int.MaxValue)) != 1)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            List<Plant> availablePlants = surroundingPlants.GetSurroundingOpponentPlantsList();

            // If there are no plants, wait a long duration before checking again
            if (availablePlants.Count == 0)
            {
                yield return new WaitForSeconds(10);
                continue;
            }

            print($"switching plant ownership to {(int)teamColor}");

            availablePlants[0].PlayerParentTransform = parentPlant.PlayerParentTransform;
            availablePlants[0].Activate(teamColor);
            availablePlants[0].PlantSpreadCreep = true;

            yield return new WaitForSeconds(1);    
        }
    }

    private int GetDistanceModifier()
    {
        if (playerParentPosition == null)
        {
            Debug.LogError("Problem in PlantCreepSpread: Player parent position is null!");
            return 1;
        }

        float distance = Vector3.Distance(playerParentPosition, transform.position);

        // If the player distance is 0, return 1. If the player distance is the max radius, return 999;
        return Mathf.RoundToInt(distanceMultiplier * (maxDistanceModifier * (distance / maxDistanceModifier)));
    }
}
