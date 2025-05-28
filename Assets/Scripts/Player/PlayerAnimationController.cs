
using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    PlayerModel playerModel;
    PlayerHoldingManager holdingManager;
    PlayerCharacterController playerCharacterController;
    PlayerInputManager playerInputManager;

    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimaor;

    private void Awake()
    {
        playerModel = GetComponentInChildren<PlayerModel>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCharacterController = GetComponent<PlayerCharacterController>();
        holdingManager = GetComponent<PlayerHoldingManager>();
    }

    private void OnEnable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll += OnToggleRagdoll;
        playerInputManager.OnPerformEmote += PlayEmote;
        holdingManager.NetworkedHeldObj.OnValueChanged += UpdateHeldAnimation;
        playerCharacterController.CharacterMovement.OnJump += PlayJumpAnimation;
    }

    private void OnDisable()
    {
        GetComponent<PlayerCharacterController>().OnToggledRagdoll -= OnToggleRagdoll;
        holdingManager.NetworkedHeldObj.OnValueChanged -= UpdateHeldAnimation;
        playerCharacterController.CharacterMovement.OnJump -= PlayJumpAnimation;

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
            foreach (IKTargetPoint ikPoint in holdingManager.HeldObj.gameObject.GetComponentsInChildren<IKTargetPoint>())
            {
                if (ikPoint.IKCONSTRAINT == IKTargetsManager.IKCONSTRAINT.LeftHand || ikPoint.IKCONSTRAINT == IKTargetsManager.IKCONSTRAINT.RightHand)
                    return;
            }

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

    public void BeginSit(Vector3 sitPos, Quaternion sitRotation)
    {
        animator.Play("Sit");
        playerCharacterController.CanMove = false;
        playerCharacterController.CharacterMovement.SetControllerEnabled(false);


        playerCharacterController.CameraManager.SetPanTiltEnabled(false);
        playerCharacterController.CameraManager.SetPanTilt(new(-90, 0));

        transform.position = sitPos;
        playerModel.transform.rotation = sitRotation;
    }

    public void EndSit(Vector3 dismountPos)
    {
        networkAnimaor.SetTrigger("Unsit");

        playerCharacterController.CanMove = true;
        playerCharacterController.CharacterMovement.SetControllerEnabled(true);

        playerCharacterController.CameraManager.SetPanTiltEnabled(true);


        transform.position = dismountPos;

        networkAnimaor.ResetTrigger("Unsit");

    }

    private void FixedUpdate()
    {
        if (animator == null) return;
        if (!IsOwner) return;

        animator.SetFloat("Velocity", playerCharacterController.CharacterMovement.CurrentVelocity);
        animator.SetFloat("WalkingMultiplier", playerCharacterController.PlayerInputManager.MovementInput.y >= 0 ? 1 : -1);
    }
}
