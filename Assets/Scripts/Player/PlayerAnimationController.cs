
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    CharacterMovement characterMovement;
    PlayerInputManager playerInputManager;

    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimaor;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void OnEnable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll += OnToggleRagdoll;
    }

    private void OnDisable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll -= OnToggleRagdoll;
    }

    public void OnToggleRagdoll(bool value)
    {
        animator.enabled = !value;
    }

    private void FixedUpdate()
    {
        if (animator == null) return;
        if (!IsOwner) return;

        animator.SetFloat("Velocity", playerInputManager.MovementInput.magnitude);
    }
}
