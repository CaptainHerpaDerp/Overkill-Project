using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TeamColors;
using static TeamColors.ColorEnum;
using Players;
using Core;
using System;
using Crore;
using UnityEngine.Playables;

namespace GameManagement
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("End The Game Now (Can also be activated by pressing ESC)")]
        [SerializeField] private bool EndGameNow;

        private Dictionary<int, TEAMCOLOR> registeredPlants = new Dictionary<int, ColorEnum.TEAMCOLOR>();
        private int[] playerTeamScore = new int[5];

        private bool isGameRunning = false;

        [Header("Game Time in Seconds")]
        [SerializeField] private int gameTime;

        [Header("The Duration of the Ending Screen in Seconds")]
        [SerializeField] private int postGameTime;

        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private GameObject CharacterSelectionUIObj;

        // The currently winning player
        Player topPlayer;

        public Dictionary<Player, int> winningTimeCount = new();

        // The crown prefab and instance that will be placed on the winning player during the game 
        [SerializeField] private GameObject crownPrefab;
        private GameObject crownInstance;

        [SerializeField] private List<Player> playerList = new();

        // Create a public accessor for the player list that is not modifiable
        public List<Player> PlayerList { get { return playerList; } }
 

        // TEMP
        [SerializeField] private GameObject victoryGroup;

        [Header("The parent of the gameobject with the placement positions ordered from 1st to 4th")]
        [SerializeField] private Transform playerPositionParent;
        [SerializeField] Vector3 playerOffset;

        [Header("Player Sphere Adjustment")]
        [SerializeField] private float minSphereSize = 1;
        [SerializeField] private float maxSphereSize = 5;
        [SerializeField] private float placementSphereUpdateInterval;
        [SerializeField] private float sizeModifier;

        // The scorebar ui object
        [SerializeField] private GameObject GameUI;

        // The director to play the end game animation
        [SerializeField] PlayableDirector director;

        // An Action to tell the badge assigner to show and start assigning badges
        public Action<float> OnAssignBadges;

        //[Header("The time after the game ends before the awards should start to be given in the badge assigner")]
        private const float badgeAssignStartDelay = 5;

        [SerializeField] private bool OpenCharacterSelectOnStart;

        public Action OnGameReload, OnGameEnd;
        public Action<Player> OnPlayerAdded;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("There is more than one GameManager in the scene");
            }

            // Enable the character selection UI
            if (OpenCharacterSelectOnStart)
            CharacterSelectionUIObj?.gameObject.SetActive(true);
        }

        public void Start()
        {
            crownInstance = Instantiate(crownPrefab, Vector3.zero, Quaternion.identity);
            crownInstance.SetActive(false);

            StartCoroutine(GiveCrownToPlayer());
            StartCoroutine(AdjustPlayerPlacementSpheres());
        }

        public void AddPlayer(Player player)
        {
            playerList.Add(player);
            OnPlayerAdded?.Invoke(player);
        }

        public Player GetMostMovePlayerWinner()
        {
            Player mostMovePlayer = null;
            float mostMove = 0;

            foreach (Player player in playerList)
            {
                if (player.timeMoving > mostMove)
                {
                    mostMove = player.timeMoving;
                    mostMovePlayer = player;
                }
            }

            return mostMovePlayer;
        }

        /// <summary>
        /// Adjust the size of the player placement spheres based on their scores, allowing losing players to catch up
        /// </summary>
        /// <returns></returns>
        private IEnumerator AdjustPlayerPlacementSpheres()
        {
            yield return new WaitForSeconds(1);

            while (true)
            {
                foreach (Player player in playerList)
                {
                    // Skip red
                    if (player.TeamColor == TEAMCOLOR.RED)
                    {
                        continue;
                    }

                    float maximumScore = 0;

                    foreach (Player compPlayer in playerList)
                    {
                        if (player == compPlayer)   
                        {
                            continue;
                        }

                        maximumScore += GetScoreForPlayer(compPlayer.TeamColor);
                    }

                    float score = GetScoreForPlayer(player.TeamColor);

                    float scoreRatio = (maximumScore / score) * sizeModifier;

                    float sphereSize = minSphereSize + scoreRatio;

                    player.PlantConversionRadius = Mathf.Clamp(sphereSize, minSphereSize, maxSphereSize);
                }

                yield return new WaitForSeconds(placementSphereUpdateInterval);
            }
        }

        private IEnumerator GiveCrownToPlayer()
        {
            while (true)
            {
                if (!isGameRunning)
                {
                    yield return new WaitForSeconds(1);
                    continue;
                }

                Player winningPlayer = null;
                int winningScore = 0;

                foreach (Player player in playerList)
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

                    if (winningTimeCount.ContainsKey(winningPlayer))
                    {
                        winningTimeCount[winningPlayer]++;
                    }
                    else {
                        winningTimeCount[winningPlayer] = 1;
                    }


                    //set the crowns parent to the child with the tag "CrownPosition"
                    foreach (Transform child in topPlayer.transform)
                    {
                        if (child.CompareTag("CrownPosition"))
                        {
                            crownInstance.transform.SetParent(child);
                            crownInstance.GetComponent<WinnerCrown>().SetCrownParentPlayer(topPlayer);
                        }
                    }


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

            ScreenDarkener.Instance.DarkenScreen();
            ScreenDarkener.Instance.OnDarkened += EndGame;

            timerText.text = "0";
        }

        public int GetScoreForPlayer(TEAMCOLOR teamColor)
        {
            return playerTeamScore[(int)teamColor];
        }

        public int GetScoreForPlayer(int team)
        {
            return playerTeamScore[team];
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
                playerTeamScore[(int)teamColor]++;
            }
        }

        public void UpdatePlantOwnership(int plantID, TEAMCOLOR newTeamColor, TEAMCOLOR oldTeamColor)
        {
            if (registeredPlants.ContainsKey(plantID))
            {
                registeredPlants[plantID] = newTeamColor;

                playerTeamScore[(int)newTeamColor]++;
                // Update the score for the old team
                ScoreReceptionManager.ChangePlayerScore((int)newTeamColor, playerTeamScore[(int)newTeamColor]);

                playerTeamScore[(int)oldTeamColor]--;
                ScoreReceptionManager.ChangePlayerScore((int)oldTeamColor, playerTeamScore[(int)oldTeamColor]);
            }
            else
            {
                Debug.LogError("Plant " + plantID + " is not registered");
            }
        }

        private void EndGame()
        {
            // Mark as game not running
            isGameRunning = false;

            // Play the dropdown camera animation
            director.Play();

            // Hide the player crown
            crownInstance.SetActive(false);

            // Hide the score bar ui
            GameUI.SetActive(false);

            // Invoke endgame so the badge assigner can start assigning badges
            OnGameEnd?.Invoke();
            OnAssignBadges?.Invoke(badgeAssignStartDelay);   

            // Show the victory area
            victoryGroup.SetActive(true);

            // disable all of the player cameras and lock movement
            foreach (Player player in playerList)
            {
                player.LockMovement = true;
                player.GetComponentInChildren<Camera>().enabled = false;
            }

            // Determine Player Score Placements
            Player firstPlace = null, secondPlace = null, thirdPlace = null, fourthPlace = null;
            int firstPlaceScore = 0, secondPlaceScore = 0, thirdPlaceScore = 0, fourthPlaceScore = 0;

            // Determine the top 3 players
            foreach (Player player in playerList)
            {
                int score = GetScoreForPlayer(player.TeamColor);

                if (firstPlace == null || score > firstPlaceScore)
                {
                    fourthPlace = thirdPlace;
                    fourthPlaceScore = thirdPlaceScore;

                    thirdPlace = secondPlace;
                    thirdPlaceScore = secondPlaceScore;

                    secondPlace = firstPlace;
                    secondPlaceScore = firstPlaceScore;

                    firstPlace = player;
                    firstPlaceScore = score;
                }
                else if (secondPlace == null || score > secondPlaceScore)
                {
                    fourthPlace = thirdPlace;
                    fourthPlaceScore = thirdPlaceScore;

                    thirdPlace = secondPlace;
                    thirdPlaceScore = secondPlaceScore;

                    secondPlace = player;
                    secondPlaceScore = score;
                }
                else if (thirdPlace == null || score > thirdPlaceScore)
                {
                    fourthPlace = thirdPlace;
                    fourthPlaceScore = thirdPlaceScore;

                    thirdPlace = player;
                    thirdPlaceScore = score;
                }
                else
                {
                    fourthPlace = player;
                    fourthPlaceScore = score;               
                }
            }

            // Place the players at the appropriate positions
            if (firstPlace != null)
                firstPlace.transform.position = playerPositionParent.GetChild(0).position + playerOffset;

            if (secondPlace != null)
                secondPlace.transform.position = playerPositionParent.GetChild(1).position + playerOffset;

            if (thirdPlace != null)
                thirdPlace.transform.position = playerPositionParent.GetChild(2).position + playerOffset;

            if (fourthPlace != null)
                fourthPlace.transform.position = playerPositionParent.GetChild(3).position + playerOffset;

            // Ensure the crown is placed on the first place player
            if (firstPlace != null)
            {
                crownInstance.transform.SetParent(firstPlace.transform.GetChild(firstPlace.transform.childCount - 1));
                crownInstance.transform.localPosition = new Vector3(0, 2, 0);
            }

            // Darken the screen to show the victory routine
            StartCoroutine(DarkenCRToExit());
        }

        private IEnumerator DarkenCRToExit()
        {
            yield return new WaitForSeconds(postGameTime);

            ScreenDarkener.Instance.DarkenScreen(0.5f);

            ScreenDarkener.Instance.OnDarkened += ExitPostGame;
        }

        private void ExitPostGame()
        {
            // Disable the Game UI
            GameUI.SetActive(false);
            victoryGroup.SetActive(false);

            OnGameReload?.Invoke();

            // Clear the list of tracked players from the sound manager 
            SoundManager.Instance.ClearPlayers();

            winningTimeCount.Clear();
            winningTimeCount = new Dictionary<Player, int>();

            // Reset the player registration
            PlayerLocator.Instance.ResetPlayerRegistration();

            // Destroy all players
            foreach (Player player in playerList)
            {
                Destroy(player.gameObject);
            }

            playerList.Clear();
            playerList = new();

            crownInstance.transform.SetParent(this.transform);
        }

        /// <summary>
        /// Start the game from the character selection state
        /// </summary>
        public void ExitCharacterSelectionState()
        {
            // Enable the game UI and start the timer
            GameUI.SetActive(true);
            StartCoroutine(DoTimerCountdown());
            isGameRunning = true;
        }
            
#if UNITY_EDITOR

        private void Update()
        {
            if (EndGameNow || Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenDarkener.Instance.DarkenScreen();
                ScreenDarkener.Instance.OnDarkened += EndGame;

                EndGameNow = false;
            }
        }

#endif
    }
}