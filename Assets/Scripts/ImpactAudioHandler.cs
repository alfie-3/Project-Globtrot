using Unity.Netcode;
using UnityEngine;

public class ImpactAudioHandler : NetworkBehaviour
{
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] AudioClipData audioClipData;
    [SerializeField] float minNoiseForce = 0.4f;

    Rigidbody rigidBody;

    float previousVelocityMagnitude;

    private void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > minNoiseForce)
        {
            PlayNoise();
        }
    }

    public void PlayNoise()
    {
        ClipData clipData = audioClipData.GetClip(1, true);

        audioSource.pitch = clipData.Pitch;
        audioSource.PlayOneShot(clipData.Clip);
    }
}
