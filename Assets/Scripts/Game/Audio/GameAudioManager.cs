using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class GameAudioManager : MonoBehaviour
{
    #region Properties - UI

    [SerializeField] private AudioClip buttonPressedSfx;
    [SerializeField] private AudioClip uiElementFocused;
    [SerializeField] private AudioClip confirmSfx;

    #endregion Properties - UI

    #region Dependencies

    private static AudioBlackboard _audioBlackboard;

    private static AudioBlackboard AudioBlackboard
    {
        get
        {
            if (_audioBlackboard == null)
                _audioBlackboard = Blackboards.Instance.AudioBlackboard;
        return _audioBlackboard;
        }
    }

    private static AudioMixerGroup MasterMixerGroup => AudioBlackboard.MasterMixerGroup;
    private static AudioMixerGroup MusicMixerGroup => AudioBlackboard.MusicMixerGroup;
    private static AudioMixerGroup SfxMixerGroup => AudioBlackboard.SfxMixerGroup;

    #endregion Dependencies

    #region Singleton

    private static GameAudioManager _instance;

    public static GameAudioManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameAudioManager>();

            Assert.IsNotNull(_instance);
            return _instance;
        }
    }

    #endregion Singleton

    public static void PlayButtonPressedSFX()
        => PlaySfx(Instance.buttonPressedSfx);

    public static void PlayUIElementFocusedSFX()
        => PlaySfx(Instance.uiElementFocused);

    public static void PlayUIConfirmSFX()
        => PlaySfx(Instance.uiElementFocused);

    private static void PlaySfx(AudioClip sfxAudioClip)
    {
        Assert.IsNotNull(sfxAudioClip);
        AudioManager.PlayClipAtCameraPoint(sfxAudioClip, audioMixerGroup: SfxMixerGroup);
    }
}