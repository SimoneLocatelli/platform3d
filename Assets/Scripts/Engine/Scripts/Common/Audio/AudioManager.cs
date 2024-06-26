﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.UIElements;

[System.Serializable]
[ExecuteAlways]
public class AudioManager : BaseBehaviour
{
    #region Properties

    public AudioMixerGroup mixerGroup;

    public List<Sound> Sounds;

    private Sound soundBeingTestedInEditMode;

    [SerializeField]
    [Tooltip("Only available when not in Playing mode")]
    private bool removeAllAudioSources;

    #endregion Properties

    #region Methods

    public static AudioSource PlayClipAtPoint(string clipPath, Vector3 position, float volume = 1, string temporaryGameObjectName = null, AudioMixerGroup audioMixerGroup = null)
    {
        var clip = CustomResources.Load<AudioClip>(clipPath);
        return PlayClipAtPoint(clip, position, volume, temporaryGameObjectName, audioMixerGroup);
    }

    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1, string temporaryGameObjectName = null, AudioMixerGroup audioMixerGroup = null)
    {
        Assert.IsNotNull(clip);
        CustomAssert.IsNotNegative(volume, nameof(volume));

        temporaryGameObjectName ??= $"One shot audio - {clip.name}";
        var gameObject = new GameObject(temporaryGameObjectName);
        gameObject.transform.position = position;
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        if (audioMixerGroup != null)
            audioSource.outputAudioMixerGroup = audioMixerGroup;

        audioSource.Play();
        var destroyAfterTime = clip.length;// * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale);
        Object.Destroy(gameObject, destroyAfterTime);

        return audioSource;
    }

    public static AudioSource PlayClipAtCameraPoint(AudioClip clip, float volume = 1, string temporaryGameObjectName = null, AudioMixerGroup audioMixerGroup = null)
        => PlayClipAtPoint(clip, Camera.main.transform.position, volume, temporaryGameObjectName, audioMixerGroup);

    public static AudioSource PlayClipAtCameraPoint(string clipPath, float volume = 1, string temporaryGameObjectName = null, AudioMixerGroup audioMixerGroup = null)
       => PlayClipAtPoint(clipPath, Camera.main.transform.position, volume, temporaryGameObjectName, audioMixerGroup);

    public void Initialise(IEnumerable<Sound> sounds)
    {
        Assert.IsNotNull(sounds);
        InitialiseSoundsArray();
        ClearSounds();

        foreach (var s in sounds)
            Sounds.Add(s.Clone());
    }

    public void Play(string soundName, bool logIfMissingSound = true, bool playAllMatchingSounds = false)
    {
        DebugLogMethodEntry($"soundName - [{soundName}]");

        if (!Application.isPlaying)
        {
            Debug.LogError($"{nameof(AudioManager)} - {nameof(Play)} - Called when not in Unity Playing mode.");
            DebugLogMethodExit($"soundName - [{soundName}]");
            return;
        }

        Assert.IsNotNull(Sounds);
        CustomAssert.IsNotEmpty(Sounds, nameof(Sounds));

        // Retrieve sound clips with the same name as soundName
        var matchingSounds = Sounds.Where(c => c.Enabled && c.Name == soundName).ToList();

        if (!matchingSounds.Any())
        {
            if (logIfMissingSound)
                Debug.LogWarning("Sound: " + soundName + " not found (or none is enabled)!");

            DebugLogMethodExit($"soundName - [{soundName}]");
            return;
        }

        if (playAllMatchingSounds)
        {
            foreach (var sound in matchingSounds)
            {
                DebugLog($"Playing sound [{soundName}]");
                Play(sound);
            }
        }
        else
        {
            // Pick the only sound found or select 1 randomly
            var soundIndex = 0;

            if (matchingSounds.Count > 1)
                soundIndex = Random.Range(0, matchingSounds.Count);

            Sound sound = matchingSounds[soundIndex];

            DebugLog($"Playing sound [{soundName} #{soundIndex}]");

            Play(sound);
        }

        DebugLogMethodExit($"soundName - [{soundName}]");
    }

    private void Play(Sound sound)
    {
        Assert.IsNotNull(sound);
        if (sound.source == null)
            InitialiseSoundSource(sound);

        sound.Update();

        sound.source.Play();
    }

    public void PlayOnDestroySounds()
        => Play("OnDestroy", playAllMatchingSounds: true);

    public bool IsPlaying()
    {
        if (Sounds == null || !Sounds.Any())
            return false;

        foreach (var s in Sounds)
        {
            if (s.source == null)
                continue;

            if (s.source.isPlaying)
                return true;
        }

        return false;
    }

    public void Copy(AudioManager audioManager)
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

    private void InitialiseSoundsArray()
    {
        Sounds ??= new List<Sound>
        {
            new Sound()
        };
    }

    private void InitialiseSoundSource(Sound s)
    {
        if (s.source != null)
            return;

        s.source = gameObject.AddComponent<AudioSource>();
        s.source.outputAudioMixerGroup = mixerGroup;
    }

    #endregion Methods

    #region LifeCycle

    private void Awake()
    {
        InitialiseSoundsArray();

        foreach (Sound s in Sounds)
            InitialiseSoundSource(s);
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (removeAllAudioSources)
            {
                DoRemoveAllAudioSources();
            }

            PlaySoundInEditMode();
        }
        else
        {
            removeAllAudioSources = false;
            RemoveAudioSourcesWithoutSoundClip();
        }
    }

    private void DoRemoveAllAudioSources()
    {
        removeAllAudioSources = false;

        if (soundBeingTestedInEditMode != null && soundBeingTestedInEditMode.source != null)
            return;

        var audioSources = GetComponents<AudioSource>();

        if (audioSources == null)
            return;

        foreach (var a in audioSources)
            a.RemoveComponentImmediate();
    }

    private void RemoveAudioSourcesWithoutSoundClip()
    {
        var audioSources = GetComponents<AudioSource>();

        if (audioSources == null)
            return;

        foreach (var a in audioSources)
        {
            if (!a.isPlaying && a.clip == null)
                a.RemoveComponentImmediate();
        }
    }

    private void PlaySoundInEditMode()
    {
        if (soundBeingTestedInEditMode == null)
        {
            if (Sounds == null || !Sounds.Any())
            {
                DoRemoveAllAudioSources();
                return;
            }

            foreach (var s in Sounds)
            {
                if (!s.playOnceInEditMode)
                    s.source = null;
            }

            var soundToPlay = Sounds.FirstOrDefault(s => s.playOnceInEditMode);

            if (soundToPlay == null)
            {
                DoRemoveAllAudioSources();
                return;
            }
            else
            {
                RemoveAudioSourcesWithoutSoundClip();
            }

            soundBeingTestedInEditMode = soundToPlay;
            InitialiseSoundSource(soundBeingTestedInEditMode);
        }
        else
        {
            if (soundBeingTestedInEditMode.source == null)
            {
                DoRemoveAllAudioSources();
                soundBeingTestedInEditMode = null;
                return;
            }

            if (soundBeingTestedInEditMode.source.isPlaying)
                return;

            if (soundBeingTestedInEditMode.playOnceInEditMode)
            {
                soundBeingTestedInEditMode.Update();
                soundBeingTestedInEditMode.source.loop = false;
                soundBeingTestedInEditMode.source.Play();
            }
            else
            {
                soundBeingTestedInEditMode.source.RemoveComponentImmediate();
            }
        }
    }

    private void Reset()
    {
        InitialiseSoundsArray();
    }

    #endregion LifeCycle
}