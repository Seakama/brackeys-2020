using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script controlling the player movement
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Reference to the CharacterController
    [SerializeField] private CharacterController characterController = default;
    [SerializeField] private Camera mainCamera = default;
    // Base movement speed
    [SerializeField] private float movementSpeed = default;
    // Base speed multiplier (not sprinting or crouching)
    [SerializeField] private float baseSpeedMultiplier = default;
    // Sprint speed multiplier
    [SerializeField] private float sprintSpeedMultiplier = default;
    [SerializeField] private float maxSprintTime = default;
    [SerializeField] private float sprintRechargeSpeed = default;
    // Gravity's force
    [SerializeField] private float gravityForce = -9.81f;
    // Speed of smoothing the inputs (greater value means smoother transition)
    [SerializeField] private float smoothSpeed = default;

    // Stores the velocity of the player used to apply gravity to the player
    private Vector3 playerVelocity;
    // Movement speed multiplier (used to sprint or crouch)
    private float movementSpeedMultiplier;
    // Stores the inputs from the InputSystem
    private Vector2 inputs;
    // Is the player sprinting?
    private bool isSprinting;
    // Stores how much time left the player can sprint
    private float sprintTimeLeft;
    // Stores the inputs in a smoothed form, so that the movement is more smoothed
    private Vector2 smoothedInputs;
    // Auxiliary variable used to smooth the inputs
    private Vector2 smoothVelocity;

    private Vector3 movement;

    /// <summary>
    /// Reference to the animator controller
    /// </summary>
    [SerializeField]
    private Animator animator = default;

    /// <summary>
    /// Reference to the audio manager
    /// </summary>
    private PlayerAudio playerAudio;

    private void Start()
    {
        movementSpeedMultiplier = baseSpeedMultiplier;
        playerAudio = GetComponent<PlayerAudio>();
    }

    private void Update()
    {
        if (isSprinting)
        {
            ConsumeSprintTime();
        }
        else
        {
            RechargeSprintTime();
        }
    }  

    /// <summary>
    /// Fixed update may cause stagger issues.
    /// Update is usually recommended for non-physics interactions.
    /// Character controller is not a physics-based component like rigid body
    /// </summary>
    private void FixedUpdate()
    {
        SmoothInputs();
        Move();
    }

    /// <summary>
    /// Smooths the raw user input to achieve a smoothed movement
    /// </summary>
    private void SmoothInputs()
    {
        smoothedInputs = Vector2.SmoothDamp(smoothedInputs, inputs, ref smoothVelocity, smoothSpeed);
    }

    /// <summary>
    /// Moves the player accordingly to the given inputs and applyes the gravity
    /// </summary>
    private void Move()
    {
        // If we're grounded and the y force is less than 0, then set it back to 0f
        if (characterController.isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = 0f;
        }

        // Prevent the character to rotating to the wrong direction when there is no input
        if (inputs.x == 0f && inputs.y == 0f)
        {
            return;
        }

        // Get the forward direction for the camera
        Vector3 forward = mainCamera.transform.forward;
        // Remove the y component (we don't want the player to face down like the camera)
        forward.y = 0f;

        // Apply the new forward direction to the transform
        transform.forward = forward;

        // Create the forward movement using the smoothed user inputs and the forward direction
        Vector3 forwardMovement = transform.forward * smoothedInputs.y;
        // Create the right movement using the smoothed user inputs and the right direction
        Vector3 rightMovement = transform.right * smoothedInputs.x;

        // Create the movement direction using the forward and right directions
        movement = (forwardMovement + rightMovement) * movementSpeed * movementSpeedMultiplier * Time.deltaTime;

        // Apply the new forward direction to the transform
        transform.forward = movement;

        // Move the object
        characterController.Move(movement);
        
        ApplyGravity();
    }

    /// <summary>
    /// Applies the gravity force to the object
    /// </summary>
    private void ApplyGravity()
    {
        // Add the gravity force (negative) to the y velocity
        playerVelocity.y += gravityForce * Time.deltaTime;
        // Apply the gravity to the object
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Reduces the sprintTimeLeft by Time.deltaTime and if there is no time left, stops the sprint
    /// </summary>
    private void ConsumeSprintTime()
    {
        if (sprintTimeLeft > 0f)
        {
            sprintTimeLeft -= Time.deltaTime;
        }
        else
        {
            sprintTimeLeft = 0f;
            StopSprint();
        }
    }

    /// <summary>
    /// Increases the sprintTimeLeft until it reaches its max
    /// </summary>
    private void RechargeSprintTime()
    {
        if (sprintTimeLeft < maxSprintTime)
        {
            sprintTimeLeft += sprintRechargeSpeed * Time.deltaTime;
        }
        else
        {
            sprintTimeLeft = maxSprintTime;
        }
    }

    /// <summary>
    /// Starts the sprint state
    /// </summary>
    private void StartSprint()
    {
        if (sprintTimeLeft > 0f)
        {
            isSprinting = true;
            // Change the multiplier to sprintMultiplier
            movementSpeedMultiplier = sprintSpeedMultiplier;

            // TODO: Here change animation to sprint
            animator.SetFloat("SprintMultiplier", 1.5f);
            playerAudio.Running.Play();
            playerAudio.Footsteps.Stop();
        }
    }

    /// <summary>
    /// Stops the sprint state
    /// </summary>
    private void StopSprint()
    {
        isSprinting = false;
        // Change the multiplier to baseMultiplier
        movementSpeedMultiplier = baseSpeedMultiplier;

        // TODO: Here change animation to walk
        animator.SetFloat("SprintMultiplier", 1f);
        playerAudio.Running.Stop();
        playerAudio.Footsteps.Play();
    }

    /// <summary>
    /// Event called whenever a Move button (WASD) is pressed
    /// </summary>
    /// <param name="context">The input data</param>
    public void OnPlayerInputMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.started)
            animator.SetBool("IsWalking", true);

        if (context.canceled)
            animator.SetBool("IsWalking", false);

        // this may need context checks 
        inputs = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Event called whenever the Shift button is pressed
    /// </summary>
    /// <param name="context">The input data</param>
    public void OnPlayerInputSprint(InputAction.CallbackContext context)
    {
        // this may need context checks 
        float state = context.ReadValue<float>();

        if (state == 1f)
        {
            StartSprint();
        }
        else
        {
            StopSprint();
        }
    }
}
