using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static TeamColors.ColorEnum;

namespace Players
{
    public enum SpecialAbility
    {
        None,
        MassConversion,
        Green,
        SmokeScreen,
        LightBeam
    }

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

        // left and right triggers
        private float pushValue, specialValue;

        public bool UseMouseClick;

        [SerializeField] private TEAMCOLOR teamColor;

        [Header("Player Behaviour Settings")]

        #region Team-Specific Settings

        [SerializeField] private SpecialAbility specialAbility;

        public CreatePlants createPlants;

        // When enabled, plants will automatically spread their influence
        public bool PlantSpreadCreep;

        // The rate at which plants grow per second
        public float GrowthRate 
        {       
            get => createPlants.GrowthRate; 
            set => createPlants.GrowthRate = value;
        }

        // When enabled, plant growth rate depends on the player's proximity to an owned animal
        public bool AnimalProximityGrowth
        {
            get => createPlants.AnimalProximityGrowth;
            set => createPlants.AnimalProximityGrowth = value;
        }

        // The maximum distance the player can be from an animal for the player's growth rate to be 0
        public float MaxDistanceToAnimal
        {
            get => createPlants.MaxAnimalDistance;
            set => createPlants.MaxAnimalDistance = value;
        }

        public float DistanceMultiplier
        {
            get => createPlants.DistanceMultiplier;
            set => createPlants.DistanceMultiplier = value;
        }

        public float MinAnimalProximityGrowthRate
        {
            get => createPlants.MinAnimalProximityGrowthRate;
            set => createPlants.MinAnimalProximityGrowthRate = value;
        }

        public bool HasRemovalTrail
        {
            get => createPlants.HasRemovalTrail;
            set => createPlants.HasRemovalTrail = value;
        }

        #endregion

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
        public bool IsSpecial => specialValue > 0;

        // private PlayerControlsGamepad playerControls;
        private InputActionAsset inputAsset;
        private InputActionMap player;

        private InputAction move, aim, push, special, jump, dPadUp, dPadDown;

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

            createPlants = GetComponentInChildren<CreatePlants>();
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
            special = player.FindAction("Special"); 
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

            if (special.triggered)
            {
                //Temp
                // find child with name "RedBehaviour"
                // call Activate on that child
                GetComponentInChildren<RedSpecialBehaviour>().Activate();

            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
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
                specialValue = Input.GetMouseButton(1) ? 1 : 0;
            }
            else
            {
                pushValue = push.ReadValue<float>();
                specialValue = special.ReadValue<float>();
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
            orientation.Rotate(Vector3.up * (-lookDirection.y * rotationSpeedY * Time.fixedDeltaTime));

            // Set the orientation's X rotation to the look direction's Y rotation
            //orientation.rotation = Quaternion.Euler(lookDirection.y, orientation.eulerAngles.y, orientation.eulerAngles.z);

            //orientation.rotation = Quaternion.Euler(0, orientation.eulerAngles.y, 0);

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