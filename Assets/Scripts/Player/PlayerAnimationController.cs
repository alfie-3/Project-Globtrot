
using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    PlayerHoldingManager holdingManager;
    CharacterMovement characterMovement;
    PlayerInputManager playerInputManager;

    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimaor;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        characterMovement = GetComponent<CharacterMovement>();
        holdingManager = GetComponent<PlayerHoldingManager>();
    }

    private void OnEnable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll += OnToggleRagdoll;
        playerInputManager.OnPerformEmote += PlayEmote;
        holdingManager.NetworkedHeldObj.OnValueChanged += UpdateHeldAnimation;
        characterMovement.OnJump += PlayJumpAnimation;
    }

    private void OnDisable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll -= OnToggleRagdoll;
        holdingManager.NetworkedHeldObj.OnValueChanged -= UpdateHeldAnimation;
        characterMovement.OnJump -= PlayJumpAnimation;

    }

    public void OnToggleRagdoll(bool value)
    {
        animator.enabled = !value;
        Physics.SyncTransforms();
    }

    private void UpdateHeldAnimation(HeldObject previousValue, HeldObject newValue)
    {
        if (newValue.IsHolding)
        {
            animator.SetBool("Holding", true);
        }
        if (!newValue.IsHolding)
        {
            animator.SetBool("Holding", false);
            animator.CrossFade("Throw", 0.1f, 2);
        }
    }

    private void PlayJumpAnimation()
    {
        networkAnimaor.SetTrigger("Jump");
    }

    private void PlayEmote(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        int emoteIndex = (int)context.ReadValue<float>();
        animator.SetInteger("Emote", emoteIndex);
        networkAnimaor.SetTrigger("TriggerEmote");
    }

    private void FixedUpdate()
    {
        if (animator == null) return;
        if (!IsOwner) return;

        animator.SetFloat("Velocity", playerInputManager.MovementInput.magnitude);
    }
}
