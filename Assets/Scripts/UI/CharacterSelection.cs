using GameManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Core;
using Crore;

namespace UIElements
{
    public class CharacterSelection : MonoBehaviour
    {
        // The manager that keeps track of player joining inputs
        [SerializeField] private PlayerInputManager playerInputManager;

        PlayerManager playerManager;

        [SerializeField] Transform redSelectionGroup, greenSelectionGroup, blueSelectionGroup, purpleSelectionGroup;

        private List<PlayerInput> players = new();
        private List<InputActionMap> playerInputs = new();

        private Dictionary<int, int> playerSelectionIndexes = new();
        private Dictionary<int, bool> canPlayerSwitch = new();
        private Dictionary<int, GameObject> playerSelectionGameObjects = new();
        private Dictionary<int, bool> playerReady = new();
        private Dictionary<int, Color> characterColours;

        // A dictionary of the claimed characters by each player
        private Dictionary<int, bool> playerSelections;

        // The UI object that will be disabled when all players are ready
        [SerializeField] private GameObject parentUIObject;

        // Determines if character selection can be done
        private bool doCharacterSelection;

        private ScreenDarkener screenDarkener;

        public bool AllPlayersReady
        {
            get
            {
                return playerReady.Count > 0 && !playerReady.ContainsValue(false);
            }
        }

        private void OnEnable()
        {
            playerInputManager.onPlayerJoined += AddPlayer;
        }

        private void OnDisable()
        {
            playerInputManager.onPlayerJoined -= AddPlayer;
        }

        private void Start()
        {
            playerManager = PlayerManager.Instance;
            screenDarkener = ScreenDarkener.Instance;
            GameManager.Instance.OnGameReload += ActivateSelection;

            // On game start, allow character selection
            doCharacterSelection = true;

            playerSelections = new Dictionary<int, bool>
        {
            { 0, false },
            { 1, false },
            { 2, false },
            { 3, false }
        };

            characterColours = new Dictionary<int, Color>
        {
            { 0, Color.red },
            { 1, Color.green },
            { 2, Color.blue },
            { 3, Color.magenta }
        };
        }


        private void AddPlayer(PlayerInput player)
        {
            if (!doCharacterSelection)
                return;

            if (player == null)
            {
                Debug.LogError("Error in Characer Selection. Player Input is Null!");
                return;
            }

            players.Add(player);

            InputActionAsset inputAsset = player.actions;
            InputActionMap playerActionMap = inputAsset.FindActionMap("Controls");

            if (playerActionMap == null)
            {
                print("No action map found");
            }

            playerActionMap.Enable();

            string playerName = "";
            int playerIndex = playerInputs.Count;

            switch (playerIndex)
            {
                case 0:
                    playerName = "P1";
                    break;
                case 1:
                    playerName = "P2";
                    break;
                case 2:
                    playerName = "P3";
                    break;
                case 3:
                    playerName = "P4";
                    break;
                default:
                    Debug.LogWarning("Player name not found");
                    break;
            }

            GameObject playerSelectionObj = redSelectionGroup.Find(playerName).gameObject;
            playerSelectionObj.gameObject.SetActive(true);
            playerSelectionGameObjects.Add(playerIndex, playerSelectionObj);

            playerSelectionIndexes.Add(playerIndex, 0);
            canPlayerSwitch.Add(playerIndex, true);
            playerReady.Add(playerIndex, false);

            playerInputs.Add(playerActionMap);
        }

        private void Update()
        {
            if (!doCharacterSelection)
                return;

            DoPlayerSelecting();
            DoPlayerLockIn();

            FinalizeCharacterSelection();
        }

        private void DoPlayerLockIn()
        {
            foreach (var playerInput in playerInputs)
            {
                // If the player presses the "X" (selection) button on the gamepad or "Space" on the keyboard
                if (playerInput.FindAction("Jump").triggered)
                {
                    int playerIndex = playerInputs.IndexOf(playerInput);

                    // If the player is already ready, unready them
                    if (playerReady[playerIndex])
                    {
                        playerReady[playerIndex] = false;

                        int selectedCharacter = playerSelectionIndexes[playerIndex];
                        playerSelections[selectedCharacter] = false;

                        playerSelectionGameObjects[playerIndex].GetComponent<Image>().color = Color.white;

                        playerSelectionGameObjects[playerIndex].transform.parent.parent.GetComponentInChildren<CharacterSelectionFadeRect>().FadeOutRect();
                        return;
                    }

                    // If the player isn't already ready, and the selected character isn't already claimed, claim it                
                    if (!playerReady[playerIndex] && playerSelections[playerSelectionIndexes[playerIndex]] == false)
                    {
                        playerReady[playerIndex] = true;
                        int selectedCharacter = playerSelectionIndexes[playerIndex];
                        playerSelections[selectedCharacter] = true;

                        Color selectedColours = characterColours[selectedCharacter];
                        playerSelectionGameObjects[playerIndex].GetComponent<Image>().color = selectedColours;

                        // Set the selection object's child hierarchy index to be the first (visually appealing)
                        playerSelectionGameObjects[playerIndex].transform.SetAsFirstSibling();

                        playerSelectionGameObjects[playerIndex].transform.parent.parent.GetComponentInChildren<CharacterSelectionFadeRect>().FadeInRect();
                        return;
                    }
                }

                if (playerInput.FindAction("Deselect").triggered)
                {
                    int playerIndex = playerInputs.IndexOf(playerInput);

                    // If the player is already ready, unready them
                    if (playerReady[playerIndex])
                    {
                        playerReady[playerIndex] = false;

                        int selectedCharacter = playerSelectionIndexes[playerIndex];
                        playerSelections[selectedCharacter] = false;

                        playerSelectionGameObjects[playerIndex].GetComponent<Image>().color = Color.white;

                        playerSelectionGameObjects[playerIndex].transform.parent.parent.GetComponentInChildren<CharacterSelectionFadeRect>().FadeOutRect();
                        return;
                    }
                }
            }
        }

        private void DoPlayerSelecting()
        {
            foreach (var playerInput in playerInputs)
            {
                // If a player is moving their "movement" stick
                Vector2 playerMoveInput = playerInput.FindAction("Movement").ReadValue<Vector2>();

                InputAction dPadLeft = playerInput.FindAction("DPadLeft");
                if (dPadLeft.IsPressed())
                {
                    playerMoveInput = new Vector2(-1, 0);
                }

                InputAction dPadRight = playerInput.FindAction("DPadRight");
                if (dPadRight.IsPressed())
                {
                    playerMoveInput = new Vector2(1, 0);
                }

                if (playerMoveInput.x != 0)
                {
                    int playerIndex = playerInputs.IndexOf(playerInput);

                    if (!canPlayerSwitch[playerIndex] || playerReady[playerIndex])
                    {
                        continue;
                    }

                    canPlayerSwitch[playerIndex] = false;

                    int moveDirection = 0;
                    if (playerMoveInput.x > 0)
                    {
                        moveDirection = 1;
                    }
                    else if (playerMoveInput.x < 0)
                    {
                        moveDirection = -1;
                    }

                    switch (playerSelectionIndexes[playerIndex])
                    {
                        case 0:
                            if (moveDirection == 1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(greenSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 1;
                            }
                            else if (moveDirection == -1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(purpleSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 3;
                            }
                            break;
                        case 1:
                            if (moveDirection == 1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(blueSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 2;
                            }
                            else if (moveDirection == -1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(redSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 0;
                            }
                            break;
                        case 2:
                            if (moveDirection == 1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(purpleSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 3;
                            }
                            else if (moveDirection == -1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(greenSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 1;
                            }
                            break;
                        case 3:
                            if (moveDirection == 1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(redSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 0;
                            }
                            else if (moveDirection == -1)
                            {
                                playerSelectionGameObjects[playerIndex].transform.SetParent(blueSelectionGroup);
                                playerSelectionIndexes[playerIndex] = 2;
                            }
                            break;
                    }
                }

                else
                {
                    canPlayerSwitch[playerInputs.IndexOf(playerInput)] = true;
                }
            }
        }

        private void FinalizeCharacterSelection()
        {
            if (!AllPlayersReady)
                return;

            // Start the screen darkening process
            screenDarkener.DarkenScreen(0.5f);

            // Tell the game manager to exit the character selection state            
            screenDarkener.OnDarkened += DoGameActivation;

            doCharacterSelection = false;

            //// Reset all player selections
            //foreach (var playerSelection in playerSelections)
            //{
            //    playerSelections[playerSelection.Key] = false;
            //}

            // Make all player selection objects white
            foreach (var playerSelectionObj in playerSelectionGameObjects)
            {
                playerSelectionObj.Value.GetComponent<Image>().color = Color.white;
                playerSelectionObj.Value.SetActive(false);
            }

            // Make all player selections children of the red selection group
            foreach (var playerSelectionObj in playerSelectionGameObjects)
            {
                playerSelectionObj.Value.transform.SetParent(redSelectionGroup);
            }

            //// Reset all player ready states
            //foreach (var playerReadyState in playerReady)
            //{
            //    playerReady[playerReadyState.Key] = false;
            //}

            //// Reset all player selection indexes
            //foreach (var playerSelectionIndex in playerSelectionIndexes)
            //{
            //    playerSelectionIndexes[playerSelectionIndex.Key] = 0;
            //}
        }

        private void DoGameActivation()
        {
            playerManager.AssignPlayers(playerSelectionIndexes);
            GameManager.Instance.ExitCharacterSelectionState();
            parentUIObject.SetActive(false);
        }

        private void ActivateSelection()
        {
            // Clear all dictionaries modified by the "AddPlayer" method
            players.Clear();
            players = new();

            // Fade out all character selection objects
            foreach (GameObject playerSelectionObj in playerSelectionGameObjects.Values)
            {
                playerSelectionObj.transform.parent.parent.GetComponentInChildren<CharacterSelectionFadeRect>().ClearDarkness();
            }

            // Set all player selections to false
            for (int i = 0; i < playerSelections.Count; i++)
            {
                playerSelections[i] = false;
            }

            playerSelectionGameObjects.Clear();
            playerSelectionGameObjects = new();

            playerSelectionIndexes.Clear();
            playerSelectionIndexes = new();

            canPlayerSwitch.Clear();
            canPlayerSwitch = new();

            playerReady.Clear();
            playerReady = new();

            playerInputs.Clear();
            playerInputs = new();

            doCharacterSelection = true;
            parentUIObject.SetActive(true);
        }
    }
}