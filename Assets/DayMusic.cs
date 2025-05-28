using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class DayMusic : MonoBehaviour
{
    AudioSource musicPlayer;
    private void Awake()
    {
        musicPlayer = GetComponent<AudioSource>();
        GameStateManager.OnDayChanged += ChangeMusic;
    }

    private void ChangeMusic(int day)
    {
        musicPlayer.clip = GameStateManager.Instance.GetCurrentDayData().Music;
    }


    private void OnDestroy()
    {
        GameStateManager.OnDayChanged -= ChangeMusic;
    }
}
