using UnityEngine;
using UnityEngine.UI;

public class UIOptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private GameSettings GameSettings => Blackboards.Instance.GameBlackboard.GameSettings;

    public void OnMasterVolumeChanged()
        => GameSettings.MasterVolume = masterVolumeSlider.value;

    public void OnMusicVolumeChanged()
        => GameSettings.MusicVolume = musicVolumeSlider.value;

    public void OnSfxVolumeChanged()
        => GameSettings.SfxVolume = sfxVolumeSlider.value;

    // Start is called before the first frame update
    private void Start()
    {
        masterVolumeSlider.value = GameSettings.MasterVolume;
        musicVolumeSlider.value = GameSettings.MusicVolume;
        sfxVolumeSlider.value = GameSettings.SfxVolume;

        Blackboards.Instance.GameBlackboard.GameState.IsPaused = true;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.frameCount % 5 != 0)
            return;

        if (Input.GetKey(KeyCode.O))
        {
            musicVolumeSlider.value--;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            musicVolumeSlider.value++;
        }
    }
}