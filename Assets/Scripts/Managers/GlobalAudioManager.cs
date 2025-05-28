using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GlobalAudioManager : NetworkBehaviour
{
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] List<AudioAsset> assetList;
    Dictionary<string, AudioAsset> assetDictionary = new();

    public static GlobalAudioManager Instance = null;

    event Action<DayState> playDayStartSound = (dayState) => { if (dayState == DayState.Open) Instance.PlaySoundSynced_Rpc("DayStart"); };
    event Action<Order, int> playNewOrderSound = (order, num) => { Instance.PlaySound("NewOrder"); };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        foreach (var asset in assetList)
        {
            assetDictionary.Add(asset.Name, asset);
        }

        GameStateManager.OnDayStateChanged += playDayStartSound;
        OrderManager.OnNewOrderAdded += playNewOrderSound;
    }

    public void PlaySound(string audioClip)
    {
        AudioClip selectedClip = SelectClip(audioClip);
        audioSource.PlayOneShot(selectedClip);
    }

    [Rpc(SendTo.Everyone)]
    public void PlaySoundSynced_Rpc(string audioClip)
    {
        AudioClip selectedClip = SelectClip(audioClip);
        audioSource.PlayOneShot(selectedClip);
    }

    public AudioClip SelectClip(string clipName)
    {
        if (assetDictionary.TryGetValue(clipName, out AudioAsset asset))
        {
            return asset.Clip;
        }

        return null;
    }

    public new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayStateChanged -= playDayStartSound;
        OrderManager.OnNewOrderAdded -= playNewOrderSound;
    }
}

[System.Serializable]
public struct AudioAsset
{
    public string Name;
    public AudioClip Clip;
}
