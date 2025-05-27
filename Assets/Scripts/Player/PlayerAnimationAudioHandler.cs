using UnityEngine;

public class PlayerAnimationAudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] CharacterMovement characterMovement;
    [Space]
    [SerializeField] AudioClipData walkingAudioClipData;
    [SerializeField] AudioClipData jumpingAudioClipData;

    private void OnEnable()
    {
        characterMovement.OnJump += OnJumpSound;
    }

    public void OnJumpSound()
    {
        if (audioSource == null) return;

        ClipData clipData = jumpingAudioClipData.GetClip(0.5f, true);

        audioSource.pitch = clipData.Pitch;
        audioSource.PlayOneShot(clipData.Clip);
    }

    public void PlayFootstep()
    {
        if (audioSource == null) return;
        if (!characterMovement.IsGrounded) return;

        ClipData clipData = walkingAudioClipData.GetClip(0.5f, true);

        audioSource.pitch = clipData.Pitch;
        audioSource.PlayOneShot(clipData.Clip);
    }
}
