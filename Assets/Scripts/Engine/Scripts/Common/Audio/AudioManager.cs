using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    #region Properties

    public AudioMixerGroup mixerGroup;

    public List<Sound> Sounds;

    #endregion Properties

    #region Methods

    public static void PlayClipAtCameraPoint(AudioClip clip, float volume = 1)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }

    public static void PlayClipAtCameraPoint(string clipPath, float volume = 1)
    {
        var clip = Resources.Load<AudioClip>(clipPath);
        PlayClipAtCameraPoint(clip, volume);
    }

    public void Initialise(IEnumerable<Sound> sounds)
    {
        Assert.IsNotNull(sounds);
        InitialiseSoundsArray();
        ClearSounds();

        foreach (var s in sounds)
            Sounds.Add(s.Clone());
    }

    public AudioSource Play(string soundName, bool logIfMissingSound = true)
    {
        var matchingSounds = Sounds.Where(c => c.name == soundName).ToList();
        var matchingSoundsCount = (int)matchingSounds?.Count();

        if (matchingSoundsCount < 1)
        {
            if (logIfMissingSound) Debug.LogWarning("Sound: " + soundName + " not found!");
            return null;
        }

        Sound sound;
        if (matchingSoundsCount > 1)
        {
            var audioIndex = Random.Range(0, matchingSoundsCount);
            sound = matchingSounds[audioIndex];
        }
        else
            sound = matchingSounds.FirstOrDefault();

        Assert.IsNotNull(sound);

        if (sound.source == null)
            InitialiseSoundSource(sound);

        sound.source.volume = sound.volume * (1f + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        sound.source.pitch = sound.pitch * (1f + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));

        sound.source.Play();

        return sound.source;
    }

    internal void Copy(AudioManager audioManager)
    {
        Assert.IsNotNull(audioManager);

        InitialiseSoundsArray();

        if (audioManager.Sounds == null || audioManager.Sounds.Count == 0)
            return;

        var soundsCount = audioManager.Sounds.Count;

        Sounds = new List<Sound>();

        for (int i = 0; i < soundsCount; i++)
            Sounds[i] = audioManager.Sounds[i].Clone();
    }

    private void ClearSounds()
    {
        foreach (var s in Sounds)
            Destroy(s.source);

        Sounds.Clear();
    }

    #endregion Methods

    #region LifeCycle

    private void Awake()
    {
        InitialiseSoundsArray();

        foreach (Sound s in Sounds)
            InitialiseSoundSource(s);
    }

    private void InitialiseSoundsArray()
    {
        Sounds ??= new List<Sound>
        {
            new Sound()
        };
    }

    private void InitialiseSoundSource(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.loop = s.loop;

        s.source.playOnAwake = false;

        s.source.outputAudioMixerGroup = mixerGroup;
    }

    private void Reset()
    {
        InitialiseSoundsArray();
    }

    #endregion LifeCycle
}