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

    [Header("The delay before starting the spread coroutine, ensures spread isnt linear in time")]
    [SerializeField] private float maxStartTimeDelay;

    [Header("The delay before spreading to another plant")]
    [SerializeField] private float spreadTimeInterval;

    Plant parentPlant;
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
        surroundingPlants.teamColour = teamColor;
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
            }
        }
        else
        {
            print("starting spread");
            surroundingPlants.teamColour = teamColor;
            spreadCoroutine ??= StartCoroutine(SpreadToSurroundingPlants());
        }
    }

    private IEnumerator SpreadToSurroundingPlants()
    {
        float randTime = Random.Range(0, maxStartTimeDelay);

        yield return new WaitForSeconds(spreadTimeInterval + randTime);

        while (true)
        {
            List<Plant> availablePlants = surroundingPlants.GetSurroundingOpponentPlantsList();

            // If there are no plants, wait a long duration before checking again
            if (availablePlants.Count == 0)
            {
                yield return new WaitForSeconds(10);
                continue;
            }

            print($"switching plant ownership to {(int)teamColor}");

            availablePlants[0].Activate(teamColor);
            availablePlants[0].PlantSpreadCreep = true;

            yield return new WaitForSeconds(spreadTimeInterval);    
        }
    }
}
