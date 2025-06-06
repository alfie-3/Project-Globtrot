using System;
using System.Collections.Generic;
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
    public event Action<InputAction.CallbackContext> OnScroll = delegate { };

    public event Action<InputAction.CallbackContext> OnPerformCtrl = delegate { };

    //Interaction
    public event Action<InputAction.CallbackContext> OnInteract = delegate { };
    public event Action<InputAction.CallbackContext> OnDismantle = delegate { };
    public event Action<InputAction.CallbackContext> OnQ = delegate { };

    public event Action<InputAction.CallbackContext> OnPerformPrimary = delegate { };
    public event Action<InputAction.CallbackContext> OnPerformSecondary = delegate { };

    public event Action<InputAction.CallbackContext> OnPerformDrop = delegate { };
    public event Action<InputAction.CallbackContext> OnSprint = delegate { };

    public event Action<InputAction.CallbackContext> OnPause = delegate { };
    public Stack<IEscapeable> EscapeStack = new Stack<IEscapeable>();

    public  PlayerCameraManager CameraManager { get; private set; }

    public event Action<InputAction.CallbackContext> OnRagdoll = delegate { };

    public event Action<InputAction.CallbackContext> OnPerformEmote = delegate { };

    public event Action<InputAction.CallbackContext> OnToggleFreeCam = delegate { };
    public event Action<InputAction.CallbackContext> OnHideUI = delegate { };

    public void Awake()
    {
        inputActions = new();
        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    private void OnEnable()
    {
        inputActions.Player.Interact.performed += context => OnInteract.Invoke(context);
        inputActions.Player.PerformPrimary.performed += context => OnPerformPrimary.Invoke(context);
        inputActions.Player.PerformPrimary.canceled += context => OnPerformPrimary.Invoke(context);
        inputActions.Player.PerformSecondary.performed += context => OnPerformSecondary.Invoke(context);
        inputActions.Player.PerformSecondary.canceled += context => { OnPerformSecondary.Invoke(context); /*Debug.Log("time: " + context.duration);*/ };
        inputActions.Player.Drop.performed += context => OnPerformDrop.Invoke(context);
        inputActions.Player.Jump.performed += context => OnJump.Invoke();

        inputActions.Player.Sprint.started += context => OnSprint.Invoke(context);
        inputActions.Player.Sprint.canceled += context => OnSprint.Invoke(context);

        inputActions.Player.Scroll.performed += context => OnScroll.Invoke(context);

        inputActions.Player.Q.performed += context => OnQ(context);
        inputActions.Player.Dismantle.performed += context => OnDismantle(context);
        inputActions.Player.Snapping.performed += context => OnPerformCtrl(context);
        inputActions.Player.Ragdoll.performed += context => OnRagdoll(context);

        inputActions.Player.Emote.performed += context => OnPerformEmote.Invoke(context);

        inputActions.Special.Freecam.performed += context => OnToggleFreeCam.Invoke(context);
        inputActions.Special.HideUI.performed += context => OnHideUI.Invoke(context);
        inputActions.Special.CaptureScreenshot.performed += context => ScreenshotTool.CaptureScreenshot(3);

        inputActions.Universal.Pause.performed += ProgressEscapeStack;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Dispose();
    }


    public void ProgressEscapeStack(InputAction.CallbackContext context)
    {
        if (EscapeStack.Count == 1)
        {
            EscapeStack.Peek().Escape(this);
        }
        else
        {
            EscapeStack.Pop().Escape(this);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Only enabled movement if is the local player (to prevent other players attempting to move character they dont own)
        if (!IsLocalPlayer) return;

        inputActions.Player.Enable();
        inputActions.Universal.Enable();
        inputActions.Special.Enable();
    }

    private void Update()
    {
        MovementInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    //Used for converting movement direction to be relative to chamera direction
    public Vector3 CameraRelativeInput()
    {
        if (CameraManager == null) return Vector3.zero;

        Vector3 inputDirection = new(MovementInput.x, 0, MovementInput.y);

        Vector3 moveDir = Vector3.zero;

        //Interprects the players movement direction based on the direction of the players camera
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + CameraManager.CamTransform.eulerAngles.y;

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        moveDir *= inputDirection.magnitude;

        return moveDir;
    }

    public void SetUIInput(bool toggle)
    {
        if (toggle)
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }
        else
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }
    }

    public void SetInputEnabled(bool toggle)
    {
        if (toggle)
        {
            inputActions.Player.Enable();
            inputActions.UI.Enable();
        }
        else
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
        }
    }
}

public interface IEscapeable
{
    public void Escape(PlayerInputManager manager);
}