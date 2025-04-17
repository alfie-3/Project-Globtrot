using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInputManager))]
public class PlayerCharacterController : NetworkBehaviour
{
    public CharacterMovement CharacterMovement { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public PlayerCameraManager CameraManager { get; private set; }

    [Header("Movement")]
    public bool CanMove = false;

    public Action<bool> OnSprintingChanged = delegate { };
    [SerializeField] float sprintMultiplier = 1.4f;
    bool sprintinInputHeld;
    bool isSprinting;
    float currentMovementMultiplier = 1f;

    [field: Space]
    [field: SerializeField] public Stamina Stamina {  get; private set; }

    [field: Space]
    public Action<bool> OnToggledRagdoll = delegate { };
    bool ragdollEnabled = false;

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
        PlayerInputManager.OnRagdoll += context => ToggleRagdoll_Rpc();
    }

    private void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
        PlayerInputManager = GetComponent<PlayerInputManager>();
        CameraManager = GetComponentInChildren<PlayerCameraManager>();
        GameStateManager.OnReset += Respawn;

        Stamina.ResetStamina();

        CursorUtils.LockAndHideCusor();
    }

    private void Update()
    {
        Stamina.UpdateStamina(sprintinInputHeld && PlayerInputManager.CameraRelativeInput().magnitude > 0.001);

        if (!CanMove) return;

        if (Stamina.CurrentStamina > 0.1f && sprintinInputHeld)
        {
            currentMovementMultiplier = sprintMultiplier;

            if (!isSprinting)
            {
                OnSprintingChanged.Invoke(true);
                isSprinting = true;
            }
        }
        else
        {
            currentMovementMultiplier = 1;

            if (isSprinting)
            {
                OnSprintingChanged.Invoke(false);
                isSprinting = false;
            }
        }

        CharacterMovement.Move(PlayerInputManager.CameraRelativeInput(), currentMovementMultiplier);
    }

    private void Jump()
    {
        CharacterMovement.Jump(PlayerInputManager.CameraRelativeInput());
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            sprintinInputHeld = true;
        else if (context.canceled)
            sprintinInputHeld = false;
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleRagdoll_Rpc()
    {
        ragdollEnabled = !ragdollEnabled;
        OnToggledRagdoll.Invoke(ragdollEnabled);
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
