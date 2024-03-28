using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : BaseBehaviour
{
    #region Private Fields

    private const float volumeDeltaFromZero = 80;

    #endregion Private Fields

    #region Audio Settings

    [SerializeField] private AudioMixer audioMixer;

    #endregion Audio Settings

    #region Public Properties

    public float MasterVolume
    {
        get => GetAudioMixerFloat("masterVolume");
        set => audioMixer.SetFloat("masterVolume", value);
    }

    public float MusicVolume
    {
        get => GetAudioMixerFloat("musicVolume");
        set => audioMixer.SetFloat("musicVolume", value);
    }

    public float SfxVolume
    {
        get => GetAudioMixerFloat("sfxVolume");
        set => audioMixer.SetFloat("sfxVolume", value);
    }

    public float MasterVolumePercentage
    {
        get => CalculateVolumePercentage(MasterVolume);
        set => MasterVolume = ConvertPercentageToVolume(value);
    }

    public float MusicVolumePercentage
    {
        get => CalculateVolumePercentage(MusicVolume);
        set => MusicVolume = ConvertPercentageToVolume(value);
    }

    public float SfxVolumePercentage
    {
        get => CalculateVolumePercentage(SfxVolume);
        set => SfxVolume = ConvertPercentageToVolume(value);
    }

    private float ConvertPercentageToVolume(float volumePercentage)
    {
        volumePercentage = FloatRounding.Round(volumePercentage, 2);
       var  volume = -volumeDeltaFromZero;
        if (volumePercentage > 0)
            volume = Mathf.Log10(volumePercentage) * 20;
        
        DebugLog($"Updating volume - Perc {volumePercentage} (Log {Mathf.Log10(volumePercentage)}) - Volume {volume}");
        return volume;
    }

    #endregion Public Properties

    #region Methods

    private float GetAudioMixerFloat(string paramName)
    {
        audioMixer.GetFloat(paramName, out float value);
        return value;
    }

    private float CalculateVolumePercentage(float currentVolume)
    {
        var percentage = (currentVolume + volumeDeltaFromZero) / (volumeDeltaFromZero);
        return percentage;
    }

    #endregion Methods
}