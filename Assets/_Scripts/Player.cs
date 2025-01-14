using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5.0f;
    public float mouseSensitivity = 2.0f;
    [SerializeField]
    private float currentSpeed;
    public float swimSpeed = 3f;
    public float swimUpForce = 3f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    private float originalHeight;

    private float rotationX = 0f;

    public Transform cameraTransform;
    private CapsuleCollider playerCollider;
    private Animator anim;

    private bool isGrounded;
    private bool isSwimming = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isJumpPressed = false;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool sprintInput;
    private bool crouchInput;
    private bool swimUpInput;
    private bool swimDownInput;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        originalHeight = playerCollider.height;
        currentSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        HandleMouseLook();

        if (isSwimming)
        {
            HandleSwimming();
        }
        else
        {
            HandleMovement();
            HandleJumping();
        }
    }

    void HandleMovement()
    {
        // Adjust Speed for Sprinting or Crouching
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
            anim.SetBool("isSprinting", true);
        }
        else
        {
            currentSpeed = walkSpeed;
            anim.SetBool("isSprinting", false);
        }

        // Movement
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
        anim.SetFloat("SpeedFB", moveInput.y);
        anim.SetFloat("SpeedLR", moveInput.x);
    }

    void HandleJumping()
    {
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("Jump");
            isGrounded = false;
            isJumpPressed = false;
            isCrouching = false;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60f, 36f);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    void HandleSwimming()
    {
        float xInput = moveInput.x;
        float zInput = moveInput.y;
        float yInput = 0f;

        if (isJumpPressed) yInput += 1f;
        if (isCrouching) yInput -= 1f;

        Vector3 swimDirection = new Vector3(xInput, yInput, zInput).normalized;
        rb.velocity = transform.TransformDirection(swimDirection) * swimSpeed;
        anim.SetFloat("SpeedFB", moveInput.y);
        anim.SetFloat("SpeedLR", moveInput.x);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero; // Reset velocity when entering water
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
            rb.useGravity = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.ResetTrigger("Jump");
            anim.SetBool("isGrounded", true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("isGrounded", false);
        }
    }

    // Input Callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        isJumpPressed = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = !isSprinting;
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            isCrouching = true;
            playerCollider.height = crouchHeight;
            transform.position -= new Vector3(0, (originalHeight - crouchHeight) / 2, 0); // Adjust position
        }
        else
        {
            isCrouching = false;
            playerCollider.height = originalHeight;
            transform.position += new Vector3(0, (originalHeight - crouchHeight) / 2, 0); // Reset position
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            anim.ResetTrigger("Attack");
        }
    }
}
