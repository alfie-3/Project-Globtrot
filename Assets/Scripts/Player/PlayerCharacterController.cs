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

    float baseFriction;
    float baseAirResistance;

    [SerializeField] Rigidbody[] ragdollRigidbodies;

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
        PlayerInputManager.OnRagdoll += context => ToggleRagdoll();
    }

    private void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
        PlayerInputManager = GetComponent<PlayerInputManager>();
        CameraManager = GetComponentInChildren<PlayerCameraManager>();
        GameStateManager.OnReset += Respawn;

        Stamina.ResetStamina();

        CursorUtils.LockAndHideCusor();

        baseFriction = CharacterMovement.Friction;
        baseAirResistance = CharacterMovement.AirResistance;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        Stamina.UpdateStamina(sprintinInputHeld && PlayerInputManager.CameraRelativeInput().magnitude > 0.001);

        if (!CanMove)
        {
            CharacterMovement.Move(Vector3.zero, currentMovementMultiplier);
            return;
        }

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
        if (!CanMove) return;
        CharacterMovement.Jump(PlayerInputManager.CameraRelativeInput());
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            sprintinInputHeld = true;
        else if (context.canceled)
            sprintinInputHeld = false;
    }

    public void ToggleRagdoll()
    {
        if (!IsLocalPlayer) return;

        ragdollEnabled = !ragdollEnabled;
        SetRagdoll_Rpc(ragdollEnabled);
    }

    public void SetRagdoll(bool value)
    {
        if (!IsLocalPlayer) return;

        ragdollEnabled = value;
        SetRagdoll_Rpc(ragdollEnabled);
    }

    [Rpc(SendTo.Everyone)]
    private void SetRagdoll_Rpc(bool value)
    {
        ragdollEnabled = value;
        OnToggledRagdoll.Invoke(ragdollEnabled);

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !ragdollEnabled;
            rb.interpolation = ragdollEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        CanMove = !ragdollEnabled;
        CharacterMovement.Friction = ragdollEnabled ? baseFriction / 4 : baseFriction;
        CharacterMovement.AirResistance = ragdollEnabled ? baseAirResistance / 4 : baseAirResistance;
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
