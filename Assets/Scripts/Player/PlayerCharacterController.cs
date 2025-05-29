using System;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInputManager))]
public class PlayerCharacterController : NetworkBehaviour
{
    public CharacterMovement CharacterMovement { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public PlayerCameraManager CameraManager { get; private set; }
    public PlayerModel PlayerModel { get; private set; }

    public PlayerBathroomHandler BathroomHandler { get; private set; }

    [Header("Movement")]
    public bool CanMove = false;

    public Action<bool> OnSprintingChanged = delegate { };
    [SerializeField] float sprintMultiplier = 1.4f;
    bool sprintinInputHeld;
    public bool IsSprinting { get; private set; }
    float currentMovementMultiplier = 1f;

    [field: Space]
    [field: SerializeField] public Stamina Stamina {  get; private set; }

    [Header("Ragdoll & Slipping")]
    [SerializeField] Rigidbody[] ragdollRigidbodies;
    [SerializeField] bool knockedOut;
    [SerializeField] float slippingImmunityCooldown = 6;
    [field: Space]
    [field: SerializeField] public float SprintingSlipChance { get; private set; } = 1f;
    [field: SerializeField] public float WalkingSlipChance { get; private set; } = 0.25f;
    float slippingImmunityTimer;
    float knockoutTime;
    public bool ImmuneToSlipping { get; private set; }
    public bool ReadyToGetUp => !knockedOut && RagdollEnabled;

    public Action<bool> OnToggledRagdoll = delegate { };
    public bool RagdollEnabled { get; private set; } = false;

    float baseFriction;
    float baseAirResistance;

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
        PlayerModel = GetComponentInChildren<PlayerModel>();
        BathroomHandler = GetComponent<PlayerBathroomHandler>();
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
        UpdateKnockoutTimer();

        Movement();
        UpdateSlipImmunity();

    }

    public void Movement()
    {
        if (!CharacterMovement.Controller.enabled) return;

        if (!CanMove)
        {
            CharacterMovement.Move(Vector3.zero, currentMovementMultiplier);
            return;
        }

        if (Stamina.CurrentStamina > 0.1f && sprintinInputHeld && PlayerInputManager.MovementInput.magnitude > 0.1f)
        {
            currentMovementMultiplier = sprintMultiplier;

            if (!IsSprinting)
            {
                OnSprintingChanged.Invoke(true);
                IsSprinting = true;
            }
        }
        else
        {
            currentMovementMultiplier = 1;

            if (IsSprinting)
            {
                OnSprintingChanged.Invoke(false);
                IsSprinting = false;
            }
        }

        currentMovementMultiplier *= BathroomHandler.GetBladderSpeedBonus();

        CharacterMovement.Move(PlayerInputManager.CameraRelativeInput(), currentMovementMultiplier);
    }

    private void Jump()
    {
        if (knockedOut) return;

        if (RagdollEnabled)
        {
            StartSlippingImmunityTimer();
            ToggleRagdoll();
        }

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
        if (RagdollEnabled && knockedOut) return;

        RagdollEnabled = !RagdollEnabled;
        SetRagdoll_Rpc(RagdollEnabled);
    }

    public void SetRagdoll(bool value)
    {
        if (!IsLocalPlayer) return;

        RagdollEnabled = value;
        SetRagdoll_Rpc(RagdollEnabled);
    }

    public void Knockout(float time)
    {
        knockoutTime = time;
        knockedOut = true;
    }

    public void StartSlippingImmunityTimer()
    {
        ImmuneToSlipping = true;
        slippingImmunityTimer = slippingImmunityCooldown;
    }

    public void UpdateSlipImmunity()
    {
        if (!ImmuneToSlipping) return;

        slippingImmunityTimer -= Time.deltaTime;

        if (slippingImmunityTimer <= 0)
        {
            ImmuneToSlipping = false;
        }
    }

    public void UpdateKnockoutTimer()
    {
        if (knockedOut == false) return;

        knockoutTime -= Time.deltaTime;

        if (knockoutTime < 0)
        {
            knockedOut = false;
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetRagdoll_Rpc(bool value)
    {
        RagdollEnabled = value;
        OnToggledRagdoll.Invoke(RagdollEnabled);

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !RagdollEnabled;
            rb.interpolation = RagdollEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        Physics.SyncTransforms();

        CanMove = !RagdollEnabled;

        CharacterMovement.Friction = RagdollEnabled ? baseFriction / 4 : baseFriction;
        CharacterMovement.AirResistance = RagdollEnabled ? baseAirResistance / 4 : baseAirResistance;

        if (RagdollEnabled)
        {
            ragdollRigidbodies[0].centerOfMass = Vector3.zero;
            ragdollRigidbodies[0].inertiaTensorRotation = Quaternion.identity;
        }
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
