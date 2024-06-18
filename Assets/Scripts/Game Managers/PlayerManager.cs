using Fergicide;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static TeamColors.ColorEnum;
using Players;
using TeamColors;
using UnityEngine.Rendering;
using System;

namespace GameManagement
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        [SerializeField] private bool listenToPlayerJoin;

        private List<PlayerInput> players = new();

        [Header("Fill this list in order of colours!")]
        [SerializeField] private List<PlayerPreferences> playerPrefs = new();

        [SerializeField] private List<Transform> spawnPoints = new();

        [SerializeField] private List<DfaultsConfig> capsuleFaces = new();

        [SerializeField] private List<int> playerIncludeLayers;
        [SerializeField] private List<int> playerExcludeLayers;

        [SerializeField] private List<VolumeProfile> playerVolumeProfiles;

        private PlayerInputManager playerInputManager;
        public Action OnPlayersSelected;
        // The parent of the creature selection crystals visible to each player
        [SerializeField] private Transform selectionCrystalTransform;

        // DEBUG
        public int firstPlayerIndex = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("PlayerManager already exists in the scene. Deleting this instance.");
                Destroy(this);
            }

            playerInputManager = GetComponent<PlayerInputManager>();
        }

        private void Start()
        {
            GameManager.Instance.OnGameReload += ResetPlayers;
        }

        private void OnEnable()
        {
            playerInputManager.onPlayerJoined += AddPlayer;
        }

        private void OnDisable()
        {
            playerInputManager.onPlayerJoined -= AddPlayer;
        }

        private void AddPlayer(PlayerInput player)
        {
            print("player join");

            players.Add(player);

            // Get the device that the player is using
            InputDevice device = player.devices[0];

            Transform playerParent = player.transform;

            Player parentPlayer = playerParent.GetComponent<Player>();

            GameManager.Instance.PlayerList.Add(parentPlayer);

            parentPlayer.LockCharacter();

            if (device.description.interfaceName == "RawInput")
            {
                parentPlayer.rotationSpeedX = 10.0f;

                parentPlayer.UseMouseClick = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void ResetPlayers()
        {
            print("Players reset");
            players.Clear();
            players = new List<PlayerInput>();
            playerInputManager.EnableJoining();
        }

        public void AssignPlayers(Dictionary<int, int> playerSelections)
        {
            playerInputManager.DisableJoining();

            for (int i = 0; i < playerSelections.Count; i++)
            {
                // Retrieve the player gameobject corresponding to the player
                PlayerInput matchedInputPlayer = players[i];

                Transform playerParent = matchedInputPlayer.transform;
                Player parentPlayer = playerParent.GetComponent<Player>();

                int selectedCharacter = playerSelections[i];

                // Assign the player their values
                if (parentPlayer.createPlants == null)
                {
                    parentPlayer.createPlants = playerParent.GetComponentInChildren<CreatePlants>();
                }

                PlayerPreferences indexPrefs = playerPrefs[selectedCharacter];
                if (indexPrefs != null)
                {
                    parentPlayer.GrowthRate = indexPrefs.GrowthRate;
                    parentPlayer.PlantSpreadCreep = indexPrefs.PlantSpreadCreep;

                    parentPlayer.AnimalProximityGrowth = indexPrefs.AnimalProximityGrowth;
                    parentPlayer.MaxDistanceToAnimal = indexPrefs.MaxDistanceToAnimal;
                    parentPlayer.DistanceMultiplier = indexPrefs.DistanceMultiplier;
                    parentPlayer.MinAnimalProximityGrowthRate = indexPrefs.MinAnimalProximityGrowthRate;

                    parentPlayer.HasRemovalTrail = indexPrefs.HasRemovalTrail;
                }

                parentPlayer.TeamColor = (TEAMCOLOR)selectedCharacter;

                playerParent.transform.position = spawnPoints[selectedCharacter].position;

                PlayerLocator.Instance.RegisterPlayerOfTeam(parentPlayer.TeamColor, playerParent.transform);

                // Include the respective player's layer in the camera culling mask
                parentPlayer.playerCamera.cullingMask |= 1 << playerIncludeLayers[selectedCharacter];

                // Remove the layer respective to the player from the camera culling mask from the player exclusion layers
                parentPlayer.playerCamera.cullingMask &= ~(1 << playerExcludeLayers[selectedCharacter]);

                parentPlayer.playerCamera.GetComponent<Volume>().profile = playerVolumeProfiles[selectedCharacter];


                // Create a new selection crystal for the player
                if (selectionCrystalTransform != null)
                {
                    GameObject selectionCrystal = Instantiate(selectionCrystalTransform.GetChild(0).gameObject, selectionCrystalTransform);
                    selectionCrystal.SetActive(false);

                    foreach (Transform child in selectionCrystal.transform.GetChild(0))
                    {
                        // Set each child's mesh renderer color to the player's team color
                        child.GetComponent<MeshRenderer>().material.color = ColorEnum.GetColor(parentPlayer.TeamColor);
                        child.gameObject.layer = playerIncludeLayers[selectedCharacter];
                    }

                    parentPlayer.GetComponentInChildren<CreatureSelector>().SelectionCrystal = selectionCrystal;
                }

                parentPlayer.UnlockCharacter();
                parentPlayer.Initialize();

                // If the player count is 3, change the last player's camera settings
                if (players.Count == 3 && i == 2)
                {
                    print("appied 3rd player cam");
                    parentPlayer.playerCamera.rect = new Rect(0, 0, 1.0f, 0.5f);
                }
            }
            OnPlayersSelected?.Invoke();

        }
    }
}