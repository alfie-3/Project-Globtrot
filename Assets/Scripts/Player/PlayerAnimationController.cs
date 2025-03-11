
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    CharacterMovement characterMovement;

    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimaor;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (animator == null) return;
        if (!IsOwner) return;

        animator.SetFloat("Velocity", characterMovement.CurrentVelocity);
    }
}
