using Unity.Netcode;
using UnityEngine;

public class Zeebo : NetworkBehaviour, IUsePrimary
{

    //[SerializeField] AnimationClip squish;
    [SerializeField] Animator zeeboAnimator;
    [SerializeField] AudioSource audioSrc;

    public void UsePrimary(PlayerHoldingManager manager)
    {
        Squish_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Squish_Rpc()
    {
        zeeboAnimator.Play("Squish");
        audioSrc.pitch = Random.Range(0.9f, 1.1f);
        audioSrc.Play();
    }

}
