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
    //public Camera playerCamera;

    private Vector2 movementInput;
    private Vector2 lookInput;

    private Vector3 playerVelocity;

    Vector2 lookDirection;
    Vector3 moveDirection;
    private float pushValue;

    public int PlayerNumber { get; set; }

    public bool IsPushing => pushValue > 0;

   // private PlayerControlsGamepad playerControls;
    private InputActionAsset inputAsset;
    private InputActionMap player;

    private InputAction move, aim, push;

    [SerializeField] private Rigidbody rb;

    [SerializeField] public Color PlayerColor { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       //playerControls = new PlayerControlsGamepad();
       
        inputAsset = GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Controls");

        Application.targetFrameRate = 120;
    }

    private void OnEnable()
    {
        move = player.FindAction("Movement");   
        aim = player.FindAction("Aim");
        push = player.FindAction("Push");

        player.Enable();
    }

    private void OnDisable()
    {
        player.Disable();
    }

    void Update()
    {
        GetInput();

        LimitSpeed();
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

    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight, groundMask))
        {
            return true;
        }

        return false;
    }

    private void GetInput()
    {
        movementInput = move.ReadValue<Vector2>();
        lookInput = aim.ReadValue<Vector2>();

        // Assuming the control scheme's push button is a button, detect if it's pressed
        pushValue = push.ReadValue<float>();

        // Get the direction the player should face based on the aim input
        lookDirection = new Vector2(lookInput.x, lookInput.y);
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
        transform.Rotate(Vector3.up * (lookDirection.x * rotationSpeedX * Time.fixedDeltaTime));
        //playerCamera.transform.Rotate(Vector3.right * (-lookDirection.y * rotationSpeedY));
        orientation.rotation = Quaternion.Euler(orientation.rotation.eulerAngles.x, transform.rotation.y, orientation.rotation.eulerAngles.z);

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
