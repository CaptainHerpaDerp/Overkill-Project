using Fergicide;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static TeamColors.ColorEnum;
using Players;
using TeamColors;

namespace GameManagement
{
    public class PlayerManager : MonoBehaviour
    {
        private List<PlayerInput> players = new();

        [Header("Fill this list in order of colours!")]
        [SerializeField] private List<PlayerPreferences> playerPrefs = new();

        [SerializeField] private List<Transform> spawnPoints = new();

        [SerializeField] private List<DfaultsConfig> capsuleFaces = new();

        [SerializeField] private List<int> playerLayers;

        private PlayerInputManager playerInputManager;

        // The parent of the creature selection crystals visible to each player
        [SerializeField] private Transform selectionCrystalTransform;

        // DEBUG
        public int firstPlayerIndex = 0;

        private void Awake()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
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
            players.Add(player);

            // Get the device that the player is using
            InputDevice device = player.devices[0];

            Transform playerParent = player.transform;

            Player parentPlayer = playerParent.GetComponent<Player>();

            ScoreManager.Instance.PlayerList.Add(parentPlayer);

            if (device.description.interfaceName == "RawInput")
            {
                parentPlayer.rotationSpeedX = 10.0f;

                parentPlayer.UseMouseClick = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (parentPlayer.createPlants == null)
            {
                parentPlayer.createPlants = playerParent.GetComponentInChildren<CreatePlants>();
            }

            PlayerPreferences indexPrefs = playerPrefs[players.Count + firstPlayerIndex - 1];
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

            parentPlayer.TeamColor = (TEAMCOLOR)players.Count + firstPlayerIndex - 1;

            playerParent.transform.position = spawnPoints[players.Count + firstPlayerIndex - 1].position;

            playerParent.GetComponentInChildren<DfaultsController>().dfaultsConfig = capsuleFaces[players.Count + firstPlayerIndex - 1];

            PlayerLocator.Instance.RegisterPlayerOfTeam(parentPlayer.TeamColor, playerParent.transform);

            // Include the respective player's layer in the camera culling mask
            parentPlayer.playerCamera.cullingMask |= 1 << playerLayers[players.Count + firstPlayerIndex - 1];

            // Create a new selection crystal for the player
            if (selectionCrystalTransform != null)
            {
                GameObject selectionCrystal = Instantiate(selectionCrystalTransform.GetChild(0).gameObject, selectionCrystalTransform);
                selectionCrystal.SetActive(false);

                foreach (Transform child in selectionCrystal.transform.GetChild(0))
                {
                   // Set each child's mesh renderer color to the player's team color
                   child.GetComponent<MeshRenderer>().material.color = ColorEnum.GetColor(parentPlayer.TeamColor);
                   child.gameObject.layer = playerLayers[players.Count + firstPlayerIndex - 1];
                }

                parentPlayer.GetComponentInChildren<CreatureSelector>().SelectionCrystal = selectionCrystal;
            }
        }
    }
}