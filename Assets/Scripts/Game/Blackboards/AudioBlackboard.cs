using UnityEngine;
using UnityEngine.Audio;

public class AudioBlackboard : BaseBlackboard
{
    [SerializeField] private AudioMixerGroup _masterMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    public AudioMixerGroup MasterMixerGroup => _masterMixerGroup;

    public AudioMixerGroup MusicMixerGroup => _musicMixerGroup;

    public AudioMixerGroup SfxMixerGroup => _sfxMixerGroup;
}