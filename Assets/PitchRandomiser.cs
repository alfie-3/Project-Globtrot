using UnityEngine;

public class PitchRandomiser : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClipData audioClipData;


    private void Awake()
    {
        ClipData clipData = audioClipData.GetClip(1, true);

        audioSource.pitch = clipData.Pitch;
        audioSource.PlayOneShot(clipData.Clip);
    }
}
