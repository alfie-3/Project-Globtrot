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

        GameStateManager.OnDayStateChanged += (context) => { if (context == true) PlaySoundSynced_Rpc("DayStart"); };
        OrderManager.OnNewOrderAdded += (order, num) => { PlaySound("NewOrder"); };
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayStateChanged -= (context) => { if (context == true) PlaySoundSynced_Rpc("DayStart"); };
        OrderManager.OnNewOrderAdded -= (order, num) => { PlaySound("NewOrder"); };
    }
}

[System.Serializable]
public struct AudioAsset
{
    public string Name;
    public AudioClip Clip;
}
