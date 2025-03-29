using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Audio Clip Data", menuName = "AudioClips/RandomClip", order = 1)]
public class AudioClipRandomData : AudioClipData
{
    [SerializeField] protected List<AudioClip> AudioClips = new();

    public override ClipData GetClip(float pitch = 1, bool randomisePitch = false)
    {
        if (randomisePitch)
        {
            pitch = GetPitchShift(pitch);
        }

        if (AudioClips.Count == 0) return new(DefaultAudioClip, pitch);

        return new(AudioClips[UnityEngine.Random.Range(0, AudioClips.Count)], pitch);
    }
}
