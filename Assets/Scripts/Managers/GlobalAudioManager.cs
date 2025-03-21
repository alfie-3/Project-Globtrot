using Unity.Netcode;
using UnityEngine;

public class GlobalAudioManager : NetworkBehaviour
{
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] AudioClip dayStart;

    private void Awake()
    {
        GameStateManager.OnDayStateChanged += (context) => { if (context == true) PlaySound(GlobalAudioClip.DayStart); };
    }

    public void PlaySound(GlobalAudioClip audioClip)
    {
        AudioClip selectedClip = SelectClip(audioClip);
        audioSource.PlayOneShot(selectedClip);
    }

    [Rpc(SendTo.Everyone)]
    public void PlaySoundSynced_Rpc(GlobalAudioClip audioClip)
    {
        AudioClip selectedClip = SelectClip(audioClip);
        audioSource.PlayOneShot(selectedClip);
    }

    public AudioClip SelectClip(GlobalAudioClip globalAudioClip)
    {
        AudioClip clip;

        switch (globalAudioClip)
        {
            case GlobalAudioClip:
                clip = dayStart;
                break;
        }

        return clip;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayStateChanged -= (context) => { if (context == true) PlaySound(GlobalAudioClip.DayStart); };
    }
}

public enum GlobalAudioClip
{
    DayStart
}
