using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public PlayerController controller;
    ThirdPersonInputs inputs;

    protected override void Awake()
    {
        base.Awake();
        inputs = new ThirdPersonInputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.SetCallbacks(controller);
        //inputs.Player.Move.performed += controller.OnMove;
        //inputs.Player.Move.canceled += controller.MoveCancelled;
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.RemoveCallbacks(controller);
        //inputs.Player.Move.performed -= controller.OnMove;
        //inputs.Player.Move.canceled -= controller.MoveCancelled;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
