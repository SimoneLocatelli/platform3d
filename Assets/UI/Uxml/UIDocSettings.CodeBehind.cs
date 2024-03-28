using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class UIDocSettings : BaseVisualElement
{
    private readonly string[] LabelClasses = new[] { CssClasses.LabelText, "textAlignLeft" };

    private AudioClip ButtonSoundEffect;

    public CustomButtonsContainer ButtonsContainer { get; private set; }

    public DropdownField DropDownResolution { get; private set; }
    public SliderInt SliderMasterVolume { get; private set; }

    public SliderInt SliderMusicVolume { get; private set; }

    public SliderInt SliderSfxVolume { get; private set; }
    public Toggle ToggleFullscreen { get; private set; }

    public CustomButtonsContainer SettingsPopupButtonsContainer { get; private set; }

    public Resolution SelectedResolution => Screen.resolutions[DropDownResolution.index];

    public bool FullscreenEnabled => ToggleFullscreen.value;

    public CustomButton ButtonSaveChanges => SettingsPopupButtonsContainer.GetButton(0);

    public bool CanPlaySoundEffects = false;

    [Preserve]
    public new class UxmlFactory : UxmlFactory<UIDocSettings>
    {
    }

    public UIDocSettings()
        : base("UIDocConfirmExitPopup",
               CssClasses.Popup)
    {
        this.LoadStyleSheet(StyleSheetsNames.GlobalStyle);
        this.LoadStyleSheet(StyleSheetsNames.USSSettings);
        this.LoadStyleSheet(StyleSheetsNames.Sliders);
        this.LoadStyleSheet(StyleSheetsNames.USSSettings);

        this.AddNewElement<VisualElement>("Menu Background", "darkBackground");

        var popupBody = this.AddNewElement<VisualElement>("Popup Body",
                                                          CssClasses.PopupBody,
                                                          CssClasses.BackgroundBorderPrimary,
                                                          CssClasses.Container_Vertical);

        popupBody.AddLabel("Settings", CssClasses.LabelTitle, "Label Settings Title");

        // Volume Sliders
        SliderMasterVolume = AddVolumeSlider(popupBody, "Master Volume");
        SliderMusicVolume = AddVolumeSlider(popupBody, "Music Volume");
        SliderSfxVolume = AddVolumeSlider(popupBody, "SFX Volume");

        // Resolution Dropdown
        DropDownResolution = AddSetting<DropdownField>(popupBody, "Display Resolution", "Dropdown Resolution", "dpdResolution");
        DropDownResolution.choices = Screen.resolutions.Select(resolution => $"{resolution.width}x{resolution.height}").ToList();
        DropDownResolution.index = Screen.resolutions.Select((resolution, index) => (resolution, index))
                                                     .First((value) => value.resolution.width == Screen.currentResolution.width && value.resolution.height == Screen.currentResolution.height)
                                                     .index;
        DropDownResolution.RegisterCallback<FocusInEvent>(_ => PlayUIElementFocusedSFX());

        // Toggle Fullscreen
        ToggleFullscreen = AddSetting<Toggle>(popupBody, "Fullscreen", "Toggle Resolution", "tgglFullscreen");
        ToggleFullscreen.value = Screen.fullScreen;
        UpdateToggleImage(ToggleFullscreen, ToggleFullscreen.value);
        ToggleFullscreen.RegisterValueChangedCallback(evt => PlayUIElementFocusedSFX());
        ToggleFullscreen.RegisterCallback<FocusInEvent>(evt => PlayUIElementFocusedSFX());

        // Exit Button
        var buttons = new List<CustomButton> { new("Return To Main Menu", UIMenuEnum.ExitMenu) };
        buttons[0].SetArrowSizeToSmall();

        SettingsPopupButtonsContainer = new CustomButtonsContainer(buttons, CustomButtonsContainer.Orientation.Horizontal, backgroundVisible: false);
        popupBody.Add(SettingsPopupButtonsContainer);

        //
        InitControlsForUIBuilder();
    }

    private TSettingControl AddSetting<TSettingControl>(VisualElement parent, string labelText, string settingControlName, params string[] settingControlClasses)
        where TSettingControl : VisualElement, new()
    {
        var settingContainer = parent.AddNewElement<VisualElement>($"Setting {labelText} Container", "settingContainer");
        settingContainer.AddLabel(labelText, LabelClasses);
        var settingControl = settingContainer.AddNewElement<TSettingControl>(settingControlName, settingControlClasses);

        return settingControl;
    }

    private SliderInt AddVolumeSlider(VisualElement parent, string labelText)
    {
        var slider = AddSetting<SliderInt>(parent, labelText, $"Slider {labelText}", "sliderVolume");

        slider.RegisterCallback<ChangeEvent<int>>(_ => PlaySliderInteractionSound());

        slider.RegisterCallback<FocusInEvent>(_ => PlayUIElementFocusedSFX());

        slider.highValue = 20;
        slider.lowValue = 0;

        return slider;
    }

    private void PlayUIElementFocusedSFX()
    {
        if (CanPlaySoundEffects)
            GameAudioManager.PlayUIElementFocusedSFX();
    }

    private void PlaySliderInteractionSound()
    {
        if (CanPlaySoundEffects)
            GameAudioManager.PlayUIElementFocusedSFX();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void InitControlsForUIBuilder()
    {
        if (Application.isPlaying)
            return;
        SliderMasterVolume.value = SliderMasterVolume.highValue;
        SliderMusicVolume.value = SliderMusicVolume.highValue / 2;
        SliderSfxVolume.value = SliderSfxVolume.lowValue;
        ToggleFullscreen.value = true;
    }

    private void UpdateToggleImage(Toggle toggle, bool isChecked)
    {
        if (isChecked)
            toggle.AddToClassList("checked");
        else
            toggle.RemoveFromClassList("checked");
    }
}