using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputManager : NetworkBehaviour
{
    //Reads player inputs and outputs them to be acccessed by other scripts that require the players inputs

    //Player input actions generated from input system data 
    InputSystem_Actions inputActions;

    public Vector2 MovementInput { get; private set; }
    public Vector2 ScrollInput { get; private set; }

    public Action OnInteract = delegate { };
    public Action OnPerformPrimary = delegate { };
    public Action OnPerformSecondary = delegate { };
    public Action<float> OnRotate = delegate { };
    

    PlayerCameraManager cameraManager;

    public void Awake()
    {
        inputActions = new();
        cameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Only enabled movement if is the local player (to prevent other players attempting to move character they dont own)
        if (IsLocalPlayer)
            inputActions.Enable();
    }

    private void Update()
    {
        MovementInput = inputActions.Player.Move.ReadValue<Vector2>();
        ScrollInput = inputActions.Player.Scroll.ReadValue<Vector2>();

        if (inputActions.Player.Interact.WasPerformedThisFrame())
            OnInteract.Invoke();

        if (inputActions.Player.PerformPrimary.WasPerformedThisFrame())
            OnPerformPrimary.Invoke();

        if (inputActions.Player.PerformSecondary.WasPerformedThisFrame())
            OnPerformSecondary.Invoke();
        Debug.Log(ScrollInput.y);
        if(ScrollInput.y != 0)
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
