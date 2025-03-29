using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Clip Data", menuName = "AudioClips/StandardClip", order = 0)]
public class AudioClipData : ScriptableObject
{
    [SerializeField] protected AudioClip DefaultAudioClip;

    int[] pentatonicSemitones = new[] { 0, 2, 4, 7, 9 };

    public virtual ClipData GetClip(float pitch = 1, bool randomisePitch = false)
    {
        if (randomisePitch)
        {
            pitch = GetPitchShift(pitch);
        }

        return new(DefaultAudioClip, pitch);
    }

    public float GetPitchShift(float inputPitch = 1)
    {
        int semitoneIndex = UnityEngine.Random.Range(0, pentatonicSemitones.Length);
        return (inputPitch * Mathf.Pow(1.059463f, pentatonicSemitones[semitoneIndex]));
    }
}

public struct ClipData
{
    public float Pitch;
    public AudioClip Clip;

    public ClipData(AudioClip clip, float pitch)
    {
        Pitch = pitch;
        Clip = clip;
    }
}