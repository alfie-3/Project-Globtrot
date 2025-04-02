using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInputManager))]
public class PlayerCharacterController : NetworkBehaviour
{
    public CharacterMovement CharacterMovement { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public PlayerCameraManager CameraManager { get; private set; }

    public bool CanMove = false;

    [SerializeField] float sprintMultiplier = 1.4f;
    float currentMovementMultiplier = 1f;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer) return;
        CanMove = true;
        CharacterMovement.Controller.enabled = true;
    }

    private void OnEnable()
    {
        PlayerInputManager.OnJump += Jump;
        PlayerInputManager.OnSprint += ToggleSprint;
    }

    private void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
        PlayerInputManager = GetComponent<PlayerInputManager>();
        CameraManager = GetComponentInChildren<PlayerCameraManager>();
        GameStateManager.OnReset += Respawn;

        CursorUtils.LockAndHideCusor();
    }

    private void Update()
    {
        if (!CanMove) return;

        CharacterMovement.Move(PlayerInputManager.CameraRelativeInput(), currentMovementMultiplier);
    }

    private void Jump()
    {
         CharacterMovement.Jump(PlayerInputManager.CameraRelativeInput());
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            currentMovementMultiplier = sprintMultiplier;
        else if (context.canceled)
            currentMovementMultiplier = 1f;
    }

    public void Respawn()
    {
        if (!IsLocalPlayer) return;

        if (SpawnManager.Instance)
        {
            SpawnManager.Instance.RespawnPlayer(this);
        }
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnReset -= Respawn;
    }
}
