using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Jukebox : BaseBehaviour
{
    #region Properties - Jukebox Songs

    [Header("Jukebox Songs")]
    [SerializeField] private List<AudioClip> Songs;

    #endregion Properties - Jukebox Songs

    #region Properties - Jukebox Settings

    [Header("Jukebox Settings")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [SerializeField, Range(0, 1)] private float maxVolume = 1;

    [SerializeField] private bool fadeIn;

    [Tooltip("Sets how many seconds will pass before the music starts playing")]
    [SerializeField, Range(0, 60)] private float startDelay = 0;

    [Tooltip("Sets how long the music fade in will be in seconds")]
    [SerializeField, Range(0, 60)] private float fadeInLength = 5;

    [Tooltip("When true, the song will start at a random timestamp within the track length")]
    [SerializeField] private bool SetRandomInitialPosition = true;

    [SerializeField] private bool PreserveJukeboxInBetweenScenes = false;


    private float initialTime = 0;

    #endregion Properties - Jukebox Settings

    #region Properties - Info

    private float playTime = -1;

    [SerializeField, ReadOnlyProperty] private float startTime = -1;
    [SerializeField, Range(0, 1)] private float volume = 1;

    #endregion Properties - Info

    #region Dependencies

    [SerializeField, ReadOnlyProperty] private AudioSource _audioSource;

    private AudioSource AudioSource => GetInitialisedComponent(ref _audioSource);

    #endregion Dependencies

    #region Methods

    private AudioClip GetRandomSong()
    {
        var index = Random.Range(0, Songs.Count - 1);

        return Songs[index];
    }

    private void Start()
    {
        if (FindObjectsOfType<Jukebox>().Count() > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (PreserveJukeboxInBetweenScenes)
            DontDestroyOnLoad(this);

        AudioSource.outputAudioMixerGroup = musicMixerGroup;

        if (fadeIn)
            AudioSource.volume = 0;
        else
            AudioSource.volume = maxVolume;

        if (startTime < 0)
            startTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        if (AudioSource.isPlaying)
        {
            if (fadeIn)
            {
                var elapsedTime = Time.timeSinceLevelLoad - playTime;
                volume = elapsedTime / fadeInLength;
                AudioSource.volume = volume;

                if (volume >= this.maxVolume)
                    fadeIn = false;
            }

            AudioSource.volume = volume;
        }
        else
        {
            if (startDelay > 0 && (Time.timeSinceLevelLoad + initialTime - startTime) < startDelay)
                return;

            playTime = Time.timeSinceLevelLoad;
            AudioSource.clip = GetRandomSong();

            if (SetRandomInitialPosition)
                initialTime = Random.Range(0, AudioSource.clip.length);

            AudioSource.time = initialTime;
            AudioSource.Play();
        }
    }

    #endregion Methods
}