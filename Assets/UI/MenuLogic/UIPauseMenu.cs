using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIPauseMenu : UIMenuLogicBase
{
    private UISettings uiSettings;
    private UIPopupConfirmExit uiPopupConfirmExit;
    private int _selectedButtonIndex = 0;
    private IEnumerable<CustomButton> buttons;
    private bool hasFocus;
    private CustomButtonsContainer MainMenuButtonsContainer;

    //private UCCustomButton SelectedButton => buttons[SelectedButtonIndex];

    public bool SubMenuOpen;

    //private int SelectedButtonIndex
    //{
    //    get => _selectedButtonIndex;
    //    set
    //    {
    //        var buttonsLength = buttons.Count();

    //        value %= buttonsLength;
    //        if (value < 0)
    //            value += buttonsLength;

    //        _selectedButtonIndex = value;
    //    }
    //}

    private IEnumerable<CustomButton> GetMenuButtons()
    {
        var document = GetComponent<UIDocument>();
        var mainMenu = document.rootVisualElement.Q<UIDocPauseMenu>();
        var rootVisualElement = document.rootVisualElement;
        rootVisualElement.Focus();

        MainMenuButtonsContainer = mainMenu.MainMenuButtonsContainer;
        var buttons = MainMenuButtonsContainer.Buttons;
        return buttons;
    }

    private void OnButtonClicked(UIMenuEnum commandId)
    {
        if (SubMenuOpen || !hasFocus)
            return;

        DebugLog("Button Clicked - Command: " + commandId);

        switch (commandId)
        {
            case UIMenuEnum.Resume:
                ResumeGame();
                break;

            case UIMenuEnum.Options:
                MainMenuButtonsContainer.UpdateButtonsFocusability(false);
                uiSettings = CustomResources.InstantiatePrefab<UISettings>(GameResources.Prefabs.UIToolkit.UISettings);
                uiSettings.OnMenuExit += () =>
                {
                    MainMenuButtonsContainer.UpdateButtonsFocusability(true);
                    MainMenuButtonsContainer.FocusButtonAtIndex(1);
                };
                break;

            case UIMenuEnum.Exit:
                MainMenuButtonsContainer.UpdateButtonsFocusability(false);
                uiPopupConfirmExit = CustomResources.InstantiatePrefab<UIPopupConfirmExit>(GameResources.Prefabs.UIToolkit.UIPopupConfirmExit);
                uiPopupConfirmExit.OnMenuExit += () =>
                {
                    MainMenuButtonsContainer.UpdateButtonsFocusability(true);
                    MainMenuButtonsContainer.FocusButtonAtIndex(2);
                };
                break;


            default:
                Assert.IsTrue(false, $"Command '{commandId}' is not supported or recognised.");
                break;
        }
    }

    private void ResumeGame()
    {
        Blackboards.Instance.GameBlackboard.GameState.StopPause();
        this.Destroy();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        buttons = GetMenuButtons().ToList();
        Assert.IsNotNull(buttons);
        foreach (var btt in buttons)
            btt.OnClick += OnButtonClicked;

        MainMenuButtonsContainer.FocusButtonAtIndex(0);
    }

    private void Start()
    {
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        hasFocus = true;
    }


    private void Update()
    {
        if (uiSettings != null || uiPopupConfirmExit != null)
            return;
        if (Blackboards.Instance.PlayerBlackboard.PlayerInputManager.PausePressedDown)
            ResumeGame();

    }
}