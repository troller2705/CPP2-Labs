using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ThirdPersonInputs.IPlayerActions
{
    [Header("Player Movement Settings")]
    private CharacterController cc;
    private ThirdPersonInputs inputs;
    private Animator anim;

    private Vector2 direction;
    private Vector3 velocity;
    
    public float normSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float crouchSpeed = 2.5f;
    public float curSpeed;
    public float jumpForce = 8.0f;   // Increased jump force
    private float gravity = -20.0f;   // Static gravity

    public float swimSpeed = 3f;
    public float swimUpForce = 3f;

    private bool isGrounded;
    private bool isSwimming = false;
    private bool isJumpPressed = false;
    private bool isAttackPressed = false;
    private bool isInteractPressed = false;
    private bool isCrouchPressed = false;
    private bool isSprintToggled = false;
    private bool isCrouchToggled = false;

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
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        curSpeed = normSpeed;
    }

    private void FixedUpdate()
    {
        HandleCamera();
        anim.SetBool("isSwimming", isSwimming);
        if (isSwimming)
        {
            HandleSwimming();
        }
        else
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        isGrounded = cc.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Keeps the player grounded
            anim.ResetTrigger("Jump");
            anim.SetBool("isGrounded", isGrounded);
        }

        // Movement input
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
        moveDirection = Quaternion.Euler(0, yaw, 0) * moveDirection;
        cc.Move(moveDirection * curSpeed * Time.fixedDeltaTime);

        // Jumping
        if (isJumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
            anim.SetTrigger("Jump");
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;

        cc.Move(velocity * Time.fixedDeltaTime);
        anim.SetFloat("SpeedFB", direction.y);
        anim.SetFloat("SpeedLR", direction.x);
    }

    void HandleSwimming()
    {
        float xInput = direction.x;
        float zInput = direction.y;
        float yInput = 0f;

        if (isJumpPressed && transform.position.y < -3) yInput += swimUpForce;
        if (isCrouchPressed) yInput -= swimUpForce;
        if (transform.position.y > -3) yInput -= swimUpForce; 

        Vector3 swimDirection = new Vector3(xInput, yInput, zInput).normalized;
        swimDirection = Quaternion.Euler(0, yaw, 0) * swimDirection;
        cc.Move(swimDirection * swimSpeed * Time.fixedDeltaTime);
        anim.SetFloat("SpeedFB", direction.y);
        anim.SetFloat("SpeedLR", direction.x);
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
        if (isSwimming)
        {
            isCrouchPressed = context.ReadValueAsButton();
        }
        else
        {
            isCrouchToggled = !isCrouchToggled;
        }
        
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprintToggled = !isSprintToggled;
        anim.SetBool("isSprinting", isSprintToggled);

        if (isSprintToggled)
        {
            curSpeed = sprintSpeed;
        }
        else
        {
            curSpeed = normSpeed;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
        if (isAttackPressed)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            anim.ResetTrigger("Attack");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        isInteractPressed = context.ReadValueAsButton();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = true;
            cc.Move(Vector3.zero); // Reset velocity when entering water
        }
        if (other.CompareTag("Finish"))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = false;
        }
    }

    public void RestartGame()
    {
        // Restarts the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
