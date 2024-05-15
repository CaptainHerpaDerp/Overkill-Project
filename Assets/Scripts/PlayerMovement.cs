using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;

    [Header("Ground Checking")]
    public float playerHeight;
    public LayerMask groundMask;

    [Header("Rotation Settings")]
    public float rotationSpeedX, rotationSpeedY;

    public Transform orientation;
    public Camera playerCamera;

    private Vector2 movementInput;
    private Vector2 lookInput;

    private Vector3 playerVelocity;


    Vector2 lookDirection;

    Vector3 moveDirection;

    private PlayerControlsGamepad playerControls;
    private PlayerInput playerInput;

    [SerializeField] private Rigidbody rb;

    private void Awake()
    {
        playerControls = new PlayerControlsGamepad();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        Application.targetFrameRate = 120;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        GetInput();

        LimitSpeed();

        HandleMovement();

        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void LateUpdate()
    {
        //HandleRotation();
    }

    private void GetInput()
    {
        movementInput = playerControls.Controls.Movement.ReadValue<Vector2>();
        lookInput = playerControls.Controls.Aim.ReadValue<Vector2>();

        // Get the direction the player should face based on the aim input
        lookDirection = new Vector2(lookInput.x, lookInput.y) * Time.deltaTime;
    }

    private void HandleMovement()
    {
        // Get the direction the player should move based on the movement input
        Vector3 moveDirection = new(movementInput.x, 0f, movementInput.y);

        moveDirection = transform.TransformDirection(moveDirection);

        // Bad collisions - smooth movement
        transform.position += moveDirection * movementSpeed * Time.deltaTime;

        // Proper collisions - jerky movement
        //rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {

        // Rotate horizontally (around y-axis)
        transform.Rotate(Vector3.up * (lookDirection.x * rotationSpeedX));
        playerCamera.transform.Rotate(Vector3.right * (-lookDirection.y * rotationSpeedY * Time.fixedDeltaTime));

        //orientation.rotation = Quaternion.Euler(orientation.rotation.eulerAngles.x, transform.rotation.y, orientation.rotation.eulerAngles.z);

        // Handle the player camera rotation in the x-axis

        //// Rotate vertically (around x-axis)
        //float newRotationX = Mathf.Clamp(transform.eulerAngles.x - lookDirection.y, 0f, 90f); // Limit vertical rotation
        //transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(moveDirection.x, 0, moveDirection.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            moveDirection = flatVelocity.normalized * movementSpeed;
        }
    }
}
