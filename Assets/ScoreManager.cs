using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private Dictionary<int, int> registeredPlants = new Dictionary<int, int>();

    [SerializeField] private int gameTime;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is more than one ScoreManager in the scene");
        }
    }

    public void Start()
    {
        StartCoroutine(DoTimerCountdown());
    }

    public IEnumerator DoTimerCountdown()
    {
        int time = gameTime;

        while (time > 0)
        {
            timerText.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
        }

        timerText.text = "0";
    }

    public int GetScoreForPlayer(int playerNumber)
    {
        int score = 0;

        foreach (KeyValuePair<int, int> plant in registeredPlants)
        {
            if (plant.Value == playerNumber)
            {
                score++;
            }
        }

        return score;
    }

    public void RegisterPlant(int plantID, int ownerNumber)
    {
        if (registeredPlants.ContainsKey(plantID))
        {
            Debug.LogError("Plant " + plantID + " is already registered");
        }
        else
        {
            registeredPlants.Add(plantID, ownerNumber);
        }
    }

    public void UpdatePlantOwnership(int plantID, int ownerNumber)
    {
        if (registeredPlants.ContainsKey(plantID))
        {
            registeredPlants[plantID] = ownerNumber;
        }
        else
        {
            Debug.LogError("Plant " + plantID + " is not registered");
        }
    }
}
