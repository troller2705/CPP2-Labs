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

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    private float originalHeight;

    [SerializeField]
    private float currentSpeed;
    private float rotationX = 0f;

    public Transform cameraTransform;
    private CapsuleCollider playerCollider;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isJumpPressed;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        originalHeight = playerCollider.height;
        currentSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleMouseLook();
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
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Movement
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }

    void HandleJumping()
    {
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
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
        if (value.isPressed)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
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
}
