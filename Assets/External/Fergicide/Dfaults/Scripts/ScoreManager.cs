using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ColorEnum;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private Dictionary<int, TEAMCOLOR> registeredPlants = new Dictionary<int, ColorEnum.TEAMCOLOR>();

    [SerializeField] private int gameTime;
    [SerializeField] private TextMeshProUGUI timerText;

    // The currently winning player
    Player topPlayer;

    [SerializeField] private GameObject crownPrefab;
    private GameObject crownInstance;

    [SerializeField] public List<Player> PlayerList = new();

    // TEMP
    [SerializeField] private GameObject victoryGroup;
    [SerializeField] private Transform p3Pos, p2Pos, p1Pos;
    [SerializeField] Vector3 playerOffset;

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

        crownInstance = Instantiate(crownPrefab, Vector3.zero, Quaternion.identity);
        crownInstance.SetActive(false);

        StartCoroutine(GiveCrownToPlayer());
    }

    private IEnumerator GiveCrownToPlayer()
    {
        while (true)
        {
            Player winningPlayer = null;
            int winningScore = 0;

            foreach (Player player in PlayerList)
            {
                int score = GetScoreForPlayer(player.TeamColor);

                if (score > winningScore)
                {
                    winningPlayer = player;
                    winningScore = score;
                }
            }

            if (winningPlayer != null)
            {
                topPlayer = winningPlayer;
                crownInstance.SetActive(true);
                
                //set the crowns parent to the last child in the player
                crownInstance.transform.SetParent(topPlayer.transform.GetChild(topPlayer.transform.childCount - 1));

                crownInstance.transform.localPosition = new Vector3(0, 2, 0);
            }

            yield return new WaitForSeconds(1);
        }
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

        EndGame();

        timerText.text = "0";
    }

    public int GetScoreForPlayer(TEAMCOLOR teamColor)
    {
        int score = 0;

        foreach (KeyValuePair<int, ColorEnum.TEAMCOLOR> plant in registeredPlants)
        {
            if (plant.Value == teamColor)
            {
                score++;
            }
        }

        return score;
    }

    public int GetScoreForPlayer(int team)
    {
        int score = 0;

        foreach (KeyValuePair<int, TEAMCOLOR> plant in registeredPlants)
        {
            if (plant.Value == (TEAMCOLOR)team)
            {
                score++;
            }
        }

        return score;
    }

    public void RegisterPlant(int plantID, TEAMCOLOR teamColor)
    {
        if (registeredPlants.ContainsKey(plantID))
        {
            Debug.LogError("Plant " + plantID + " is already registered");
        }
        else
        {
            registeredPlants.Add(plantID, teamColor);
        }
    }

    public void UpdatePlantOwnership(int plantID, TEAMCOLOR teamColor)
    {
        if (registeredPlants.ContainsKey(plantID))
        {
            registeredPlants[plantID] = teamColor;
        }
        else
        {
            Debug.LogError("Plant " + plantID + " is not registered");
        }
    }

    private void EndGame()
    {
        victoryGroup.SetActive(true);

        // disable all of the player cameras and lock movement
        foreach (Player player in PlayerList)
        {
            player.LockMovement = true;
            player.GetComponentInChildren<Camera>().enabled = false;
        }

        Player firstPlace = null, secondPlace = null, thirdPlace = null;
        int firstPlaceScore = 0, secondPlaceScore = 0, thirdPlaceScore = 0;

        // Determine the top 3 players
        foreach (Player player in PlayerList)
        {
            int score = GetScoreForPlayer(player.TeamColor);

            if (firstPlace == null || score > firstPlaceScore)
            {
                thirdPlace = secondPlace;
                thirdPlaceScore = secondPlaceScore;

                secondPlace = firstPlace;
                secondPlaceScore = firstPlaceScore;

                firstPlace = player;
                firstPlaceScore = score;
            }
            else if (secondPlace == null || score > secondPlaceScore)
            {
                thirdPlace = secondPlace;
                thirdPlaceScore = secondPlaceScore ;

                secondPlace = player;
                secondPlaceScore = score;
            }
            else if (thirdPlace == null || score > thirdPlaceScore)
            {
                thirdPlace = player;
                thirdPlaceScore = score;
            }
        }

        // Place the players at the appropriate positions
        if (firstPlace != null)
            firstPlace.transform.position = p1Pos.position + playerOffset;

        if (secondPlace != null)
            secondPlace.transform.position = p2Pos.position + playerOffset;

        if (thirdPlace != null)
            thirdPlace.transform.position = p3Pos.position + playerOffset;

        // Ensure the crown is placed on the first place player
        if (firstPlace != null)
        {
            crownInstance.transform.SetParent(firstPlace.transform.GetChild(firstPlace.transform.childCount - 1));
            crownInstance.transform.localPosition = new Vector3(0, 2, 0);
        }
    }
}
