using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInputManager))]
public class PlayerCharacterController : NetworkBehaviour
{
    public CharacterMovement CharacterMovement { get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }

    public bool CanMove = false;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer) return;
        CanMove = true;
        CharacterMovement.Controller.enabled = true;
    }

    private void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
        PlayerInputManager = GetComponent<PlayerInputManager>();

        CursorUtils.LockAndHideCusor();
    }

    private void Update()
    {
        if (!CanMove) return;

        CharacterMovement.Move(PlayerInputManager.CameraRelativeInput());
    }
}
