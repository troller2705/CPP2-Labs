using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ThirdPersonInputs.IPlayerActions
{
    CharacterController cc;

    Vector2 direction;
    Vector3 velocity;

    float gravity;

    private float speed = 5f;
    private float jumpHeight = 5f;
    private float jumpTime = 1f;

    float timeToApex;
    float initalJumpVelocity;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        timeToApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        initalJumpVelocity = -(gravity * timeToApex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }

    public void MoveCancelled(InputAction.CallbackContext ctx)
    {
        direction = Vector2.zero;
    }

    private void FixedUpdate()
    {
        velocity = new Vector3(direction.x * speed, velocity.y, direction.y * speed);

        if (!cc.isGrounded) velocity.y += gravity * Time.fixedDeltaTime;
        else velocity.y = -cc.minMoveDistance;

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
