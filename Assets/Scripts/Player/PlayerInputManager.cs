using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : NetworkBehaviour
{
    //Reads player inputs and outputs them to be acccessed by other scripts that require the players inputs

    //Player input actions generated from input system data 
    InputSystem_Actions inputActions;

    //Movement
    public Vector2 MovementInput { get; private set; }
    public Action OnJump = delegate { };

    //Rotating
    public Vector2 ScrollInput { get; private set; }
    public Action<float> OnRotate = delegate { };

    //Interaction
    public Action<InputAction.CallbackContext> OnInteract = delegate { };
    public Action<InputAction.CallbackContext> OnDismantle = delegate { };

    public Action<InputAction.CallbackContext> OnPerformPrimary = delegate { };
    public Action<InputAction.CallbackContext> OnPerformSecondary = delegate { };

    public Action<InputAction.CallbackContext> OnPerformDrop = delegate { };
    public Action<InputAction.CallbackContext> OnSprint = delegate { };


    PlayerCameraManager cameraManager;

    public void Awake()
    {
        inputActions = new();
        cameraManager = GetComponentInChildren<PlayerCameraManager>();

        inputActions.Player.Interact.performed += context => { OnInteract.Invoke(context); };
        inputActions.Player.PerformPrimary.performed += context => { OnPerformPrimary.Invoke(context); };
        inputActions.Player.PerformSecondary.performed += context => { OnPerformSecondary.Invoke(context); Debug.Log("time: " + context.duration); };
        inputActions.Player.Drop.performed += context => { OnPerformDrop.Invoke(context); };
        inputActions.Player.Jump.performed += context => { OnJump.Invoke(); };

        inputActions.Player.Sprint.started += context => { OnSprint.Invoke(context); };
        inputActions.Player.Sprint.canceled += context => { OnSprint.Invoke(context); };

        inputActions.Player.Dismantle.performed += context => OnDismantle(context);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Only enabled movement if is the local player (to prevent other players attempting to move character they dont own)
        if (IsLocalPlayer)
            inputActions.Enable();
    }

    void OnMoveInput(InputAction.CallbackContext context)
    {

        // Get the input value from the context

        float movementValue = context.ReadValue<float>();


        //context.
        // Trigger the delegate with the input value

        //OnMove?.Invoke(movementValue);
    }

    private void Update()
    {
        MovementInput = inputActions.Player.Move.ReadValue<Vector2>();
        ScrollInput = inputActions.Player.Scroll.ReadValue<Vector2>();

        if (ScrollInput.y != 0)
            OnRotate.Invoke(ScrollInput.y);
    }

    //Used for converting movement direction to be relative to chamera direction
    public Vector3 CameraRelativeInput()
    {
        if (cameraManager == null) return Vector3.zero;

        Vector3 inputDirection = new(MovementInput.x, 0, MovementInput.y);

        Vector3 moveDir = Vector3.zero;

        //Interprects the players movement direction based on the direction of the players camera
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraManager.CamTransform.eulerAngles.y;

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        moveDir *= inputDirection.magnitude;

        return moveDir;
    }


}
