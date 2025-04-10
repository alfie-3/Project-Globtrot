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

    public Action<float> OnStaminaUpdated = delegate { };
    [SerializeField] float maxStamina = 5.0f;
    [SerializeField] float staminaDrainSpeed = 1;
    [SerializeField] float staminaRechargeSpeed = 1;
    float currentStamina;

    public Action<bool> OnSprintingChanged = delegate { };
    [SerializeField] float sprintMultiplier = 1.4f;
    bool sprintinInputHeld;
    bool isSprinting;
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

        currentStamina = maxStamina;

        CursorUtils.LockAndHideCusor();
    }

    private void Update()
    {
        UpdateStamina();

        if (!CanMove) return;

        if (currentStamina > 0.1f && sprintinInputHeld)
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

    public void UpdateStamina()
    {
        if (sprintinInputHeld && PlayerInputManager.CameraRelativeInput().magnitude > 0.1f)
        {
            currentStamina = Mathf.Clamp(currentStamina - (staminaDrainSpeed * Time.deltaTime), 0, maxStamina);

            if (currentStamina > 0.1f)
                currentMovementMultiplier = sprintMultiplier;
        }
        else
        {
            currentStamina = Mathf.Clamp(currentStamina + (staminaRechargeSpeed * Time.deltaTime), 0, maxStamina);
        }

        OnStaminaUpdated.Invoke(currentStamina / maxStamina);
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
