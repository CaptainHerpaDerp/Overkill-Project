using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new();

    [SerializeField] private List<Transform> spawnPoints = new();

    [SerializeField] private List<Color> playerColors = new();

    [SerializeField] private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;


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

        print(device.description.interfaceName);

        //Transform playerParent = player.transform.parent;
        Transform playerParent = player.transform;

        PlayerMovement playerMovement = playerParent.GetComponent<PlayerMovement>();

        if (device.description.interfaceName == "RawInput")
        {
            playerMovement.rotationSpeedX = 10.0f;

            playerMovement.UseMouseClick = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        playerMovement.PlayerColor = playerColors[players.Count - 1];
        playerMovement.PlayerNumber = players.Count;

        playerParent.transform.position = spawnPoints[players.Count - 1].position;

        // What is mathf.log??

        // Answer: Mathf.Log is a method that returns the logarithm of a specified number in a specified base.

        // Mathf.Log(8, 2) returns 3 because 2^3 = 8
       // int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2); 

        //playerParent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;

        // What is |= in C#?

        // Answer: |= is a bitwise OR assignment operator. It takes the current value of the variable on the left and the value on the right, performs a bitwise OR operation on them, and assigns the result to the variable on the left.

        // Example: a |= b is equivalent to a = a | b

     //   playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        //playerParent.GetComponentInChildren<InputHandler>.horizontal = player.actions.FindAction("Look");
    }
}
