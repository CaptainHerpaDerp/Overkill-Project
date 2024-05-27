using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static TeamColors.ColorEnum;

namespace Players
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float movementSpeed;

        [Header("Ground Checking")]
        public float playerHeight;
        public LayerMask groundMask;

        [Header("Rotation Settings")]
        public float rotationSpeedX, rotationSpeedY;

        [SerializeField] private float jumpForce;

        public Transform orientation;
        public Camera playerCamera;

        private Vector2 movementInput;
        private Vector2 lookInput;

        Vector2 lookDirection;
        Vector3 moveDirection;

        private float pushValue;

        public bool UseMouseClick;

        [SerializeField] private TEAMCOLOR teamColor;

        [Header("Player Behaviour Settings")]

        // When enabled, plants will automatically spread their influence
        [SerializeField] private bool plantSpreadCreep;
        public bool PlantSpreadCreep { get => plantSpreadCreep; }

        public float PlantConversionRadius
        {
            get
            {
                return GetComponentInChildren<CreatePlants>().GetComponent<SphereCollider>().radius;
            }

            set
            {
                GetComponentInChildren<CreatePlants>().GetComponent<SphereCollider>().radius = value;
            }
        }

        public bool LockMovement;

        public TEAMCOLOR TeamColor
        {
            get => teamColor;

            set
            {


                OnPlayerNumberChange?.Invoke();
                teamColor = value;
            }
        }

        public Action OnPlayerNumberChange;

        [SerializeField] private bool UpdatePlayerValues;

        public bool IsPushing => pushValue > 0;

        // private PlayerControlsGamepad playerControls;
        private InputActionAsset inputAsset;
        private InputActionMap player;

        private InputAction move, aim, push, jump, dPadUp, dPadDown;

        private bool doJump;

        [SerializeField] private Rigidbody rb;

        // TEMP
        private Vector3 spawnPoint;
        public Action OnPlayerRespawn;
        [SerializeField] private float fovZoomFactor;

        private const float respawnY = -10f;

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
            jump = player.FindAction("Jump");

            dPadUp = player.FindAction("DPadUp");
            dPadDown = player.FindAction("DPadDown");

            player.Enable();

            if (Gamepad.current != null)
            {
                //print(Gamepad.current.name);
            }

            spawnPoint = transform.position;
        }

        private void OnDisable()
        {
            player.Disable();
        }

        void Update()
        {
            GetInput();

            LimitSpeed();

            if (UpdatePlayerValues)
            {
                OnPlayerNumberChange?.Invoke();
                UpdatePlayerValues = false;
            }

            if (transform.position.y < respawnY)
            {
                OnPlayerRespawn?.Invoke();
                transform.position = spawnPoint;
            }

            if (doJump)
            {
                if (IsGrounded())
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }

                doJump = false;
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void SetPlayerBehaviour(TEAMCOLOR playerColor)
        {
            switch (playerColor)
            {

            }
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

            if (UseMouseClick)
            {
                pushValue = Input.GetMouseButton(0) ? 1 : 0;
            }
            else
            {
                pushValue = push.ReadValue<float>();
            }

            if (dPadDown.IsPressed())
            {
                if (playerCamera.fieldOfView < 130)
                {
                    playerCamera.fieldOfView++;

                    // Move the camera farther from the player
                    Vector3 diffNormalized = transform.position - playerCamera.transform.position;
                    playerCamera.transform.position += diffNormalized * fovZoomFactor;
                }
            }

            if (dPadUp.IsPressed())
            {
                if (playerCamera.fieldOfView > 30)
                {
                    playerCamera.fieldOfView--;

                    // Move the camera closer to the player
                    Vector3 diffNormalized = transform.position - playerCamera.transform.position;
                    playerCamera.transform.position -= diffNormalized * fovZoomFactor;
                }
            }

            // dpadValue = dPadUp.triggered ? 1 : dPadDown.triggered ? -1 : 0;

            doJump = jump.triggered;

            // Get the direction the player should face based on the aim input
            lookDirection = new Vector2(lookInput.x, lookInput.y);
        }

        private void HandleMovement()
        {
            if (LockMovement)
                return;

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
            // orientation.rotation = Quaternion.Euler(orientation.rotation.eulerAngles.x, transform.rotation.y, orientation.rotation.eulerAngles.z);

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
}