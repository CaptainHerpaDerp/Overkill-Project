using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TwinStick : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float controllerDeadZone = 0.1f;
    [SerializeField] private float gamepadRotateSmoothing = 1000f;

    [SerializeField] private bool isGamepad;

    private CharacterController controller;

    private Vector2 movement;
    private Vector2 aim;

    private Vector3 playerVelocity;

    private PlayerControls playerControls;
    private PlayerInput playerInput;

    [SerializeField] private Camera playerCamera;

    // Debugging
    [SerializeField] private bool Teleport;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();

        if (Teleport)
        {
            Teleport = false;
            transform.position = new Vector3(0, 0, 0);
        }
    }

    public void TP(Vector3 pos)
    {
        transform.position = pos;
    }

    private void HandleInput()
    {
        movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector3 playerDirection = Vector3.right * aim.x + Vector3.forward * aim.y;
        if (playerDirection.sqrMagnitude > 0.0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, gamepadRotateSmoothing * Time.deltaTime);

            playerCamera.transform.localRotation = Quaternion.Euler(newRotation.x, 0, 0);
        }

      //  rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
     //   rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

    }

    public void OnDeviceChange(PlayerInput playerInput)
    {
        isGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}
