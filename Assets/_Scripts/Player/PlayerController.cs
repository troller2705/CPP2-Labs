using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ThirdPersonInputs.IPlayerActions
{
    [Header("Player Movement Settings")]
    private CharacterController cc;
    private ThirdPersonInputs inputs;

    private Vector2 direction;
    private Vector3 velocity;
    private bool isJumpPressed = false;

    public float speed = 5.0f;
    public float jumpForce = 8.0f;   // Increased jump force
    public float gravity = -20.0f;   // Stronger gravity

    private bool isGrounded;

    [Header("Camera Settings")]
    public Transform cameraTarget;
    public Transform cameraTransform;
    public float mouseSensitivity = 2.0f;
    public float cameraDistance = 3.0f;
    public float cameraHeight = 2.0f;
    public float rotationSmoothTime = 0.1f;

    private Vector2 lookInput;
    private float yaw = 0f;
    private float pitch = 0f;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    void Awake()
    {
        inputs = new ThirdPersonInputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.RemoveCallbacks(this);
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCamera();
    }

    private void HandleMovement()
    {
        isGrounded = cc.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Keeps the player grounded
        }

        // Movement input
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
        moveDirection = Quaternion.Euler(0, yaw, 0) * moveDirection;
        cc.Move(moveDirection * speed * Time.fixedDeltaTime);

        // Jumping
        if (isJumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    private void HandleCamera()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -24.5f, 40f);

        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        cameraTarget.eulerAngles = currentRotation;

        Vector3 offset = (cameraTarget.forward * -cameraDistance) + (Vector3.up * cameraHeight);
        cameraTransform.position = cameraTarget.position + offset;
        cameraTransform.LookAt(cameraTarget);

        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    // Input Callbacks
    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) direction = ctx.ReadValue<Vector2>();
        if (ctx.canceled) direction = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Implement crouch logic here
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // Implement sprint logic here
    }
}
