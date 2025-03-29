using UnityEngine;

public class Zeebo : MonoBehaviour, IUsePrimary
{

    //[SerializeField] AnimationClip squish;
    [SerializeField] Animator zeeboAnimator;
    [SerializeField] AudioSource audioSrc;

    public void UsePrimary(PlayerHoldingManager manager)
    {
        zeeboAnimator.Play("Squish");
        audioSrc.pitch = Random.Range(0.9f, 1.1f);
        audioSrc.Play();
    }

}
