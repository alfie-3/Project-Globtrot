using UnityEngine;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInputManager))]
public class PlayerCharacterController : MonoBehaviour
{
    public CharacterMovement CharacterMovement {  get; private set; }
    public PlayerInputManager PlayerInputManager { get; private set; }

    private void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
        PlayerInputManager = GetComponent<PlayerInputManager>();

        CursorUtils.LockAndHideCusor();
    }

    private void Update()
    {
        CharacterMovement.Move(PlayerInputManager.CameraRelativeInput());
    }
}
