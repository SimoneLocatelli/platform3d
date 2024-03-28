using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UISettings : UIMenuLogicBase

{
    #region Fields

    private GameSettings gameSettings;
    private UIDocSettings uiDocSettings;
    private CursorLockMode previousCursorLockState;

    [SerializeField, Range(0, 5000)] private float delayBeforePlayingSfxSampleSound;
    private float timerBeforeBeforePlayingSfxSampleSound;
    [SerializeField] private bool HasMouseCapture;

    #endregion Fields

    #region Life Cycle

    private void Start()
    {
        // Init Variables
        gameSettings = Blackboards.Instance.GameBlackboard.GameSettings;
        var document = GetComponent<UIDocument>();
        uiDocSettings = document.rootVisualElement.Q<UIDocSettings>();

        uiDocSettings.CanPlaySoundEffects = false;

        // Register Events
        uiDocSettings.SliderMasterVolume.value = Mathf.RoundToInt(gameSettings.MasterVolumePercentage * uiDocSettings.SliderMasterVolume.highValue);
        uiDocSettings.SliderMusicVolume.value = Mathf.RoundToInt(gameSettings.MusicVolumePercentage * uiDocSettings.SliderMasterVolume.highValue);
        uiDocSettings.SliderSfxVolume.value = Mathf.RoundToInt(gameSettings.SfxVolumePercentage * uiDocSettings.SliderMasterVolume.highValue);

        uiDocSettings.SliderMasterVolume.RegisterValueChangedCallback(UpdateMasterVolumeSlider);
        uiDocSettings.SliderMusicVolume.RegisterValueChangedCallback(UpdateMusicVolumeSlider);
        uiDocSettings.SliderSfxVolume.RegisterValueChangedCallback(UpdateSfxVolumeSlider);

        uiDocSettings.DropDownResolution.RegisterValueChangedCallback(OnResolutionChanged);

        uiDocSettings.ToggleFullscreen.RegisterValueChangedCallback(OnFullscreenChanged);

        uiDocSettings.ButtonSaveChanges.OnClick += OnButtonSaveChangesClicked;

        // Allow mouse movement
        previousCursorLockState = UnityEngine.Cursor.lockState;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        GetComponent<UIDocument>().enabled = true;

        // Init Focus
        uiDocSettings.SliderMasterVolume.Focus();

        uiDocSettings.CanPlaySoundEffects = true;
    }

    private void Update()
    {
        HasMouseCapture = uiDocSettings.SliderSfxVolume.HasMouseCapture();
    }

    #endregion Life Cycle

    #region UI Event Handlers

    private void OnFullscreenChanged(ChangeEvent<bool> evt)
        => RefreshResolutionAndFullScreen();

    private void OnResolutionChanged(ChangeEvent<string> evt)
        => RefreshResolutionAndFullScreen();

    private void OnButtonSaveChangesClicked(UIMenuEnum commandId)
    {
        SaveChanges();
        ExitSettingsMenu();
    }

    private void RefreshResolutionAndFullScreen()
    {
        var selectedResolution = uiDocSettings.SelectedResolution;
        var fullscreenEnabled = uiDocSettings.FullscreenEnabled;
        var resolutionWidth = selectedResolution.width;
        var resolutionHeight = selectedResolution.height;
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreen: fullscreenEnabled);
        DebugLog($"Changed resolution to [{resolutionWidth}x{resolutionHeight} - Fullscreen: [{fullscreenEnabled}]");
    }

    private void UpdateMasterVolumeSlider(ChangeEvent<int> evt)
        => gameSettings.MasterVolumePercentage = (float)evt.newValue / uiDocSettings.SliderMusicVolume.highValue;
    private void UpdateMusicVolumeSlider(ChangeEvent<int> evt)
        => gameSettings.MusicVolumePercentage = (float)evt.newValue / uiDocSettings.SliderMusicVolume.highValue;

    private void UpdateSfxVolumeSlider(ChangeEvent<int> evt)
        => gameSettings.SfxVolumePercentage = (float)evt.newValue / uiDocSettings.SliderSfxVolume.highValue;

    #endregion UI Event Handlers

    #region Methods

    private void ExitSettingsMenu()
    {
        UnityEngine.Cursor.lockState = previousCursorLockState;
        GetComponent<UIDocument>().enabled = false;
    }

    private void SaveChanges()
    {
        RefreshResolutionAndFullScreen();
        ExitMenu();
    }

    #endregion Methods
}