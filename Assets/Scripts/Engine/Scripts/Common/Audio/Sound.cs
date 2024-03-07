using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public bool loop = false;
    public AudioMixerGroup mixerGroup;
    public string name;

    [Range(.1f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float pitchVariance;

    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float volume;

    [Range(0f, 1f)]
    public float volumeVariance;

    public Sound()
    {
        pitch = 1f;
        pitchVariance = .1f;
        volume = 1f;
        volumeVariance = .1f;
    }

    internal Sound Clone()
    {
        return new Sound
        {
            name = this.name,
            clip = this.clip,
            pitch = this.pitch,
            pitchVariance = this.pitchVariance,
            volume = this.volume,
            volumeVariance = this.volumeVariance
        };
    }
}