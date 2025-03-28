
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

    private void FixedUpdate()
    {
        if (animator == null) return;
        if (!IsOwner) return;

        animator.SetFloat("Velocity", playerInputManager.MovementInput.magnitude);
    }
}
