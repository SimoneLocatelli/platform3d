using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class UIPopupConfirmExit : UIMenuLogicBase
{
    private UIDocConfirmExitPopup UIDocConfirmExitPopup;

    private void OnButtonClicked(UIMenuEnum commandId)
    {
        DebugLog("Button Clicked - Command: " + commandId);

        switch (commandId)
        {
            case UIMenuEnum.Yes:
                
                ApplicationTerminator.ExitGame();
                break;

            case UIMenuEnum.No:
                ExitMenu();
                break;

            default:
                Assert.IsTrue(false, $"Command '{commandId}' is not supported or recognised.");
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIDocConfirmExitPopup = UIDocument.rootVisualElement.Q<UIDocConfirmExitPopup>();
        var buttons = UIDocConfirmExitPopup.ButtonsContainer.Buttons;
        Assert.IsNotNull(buttons);

        foreach (var btt in buttons)
            btt.OnClick += OnButtonClicked;

        UIDocConfirmExitPopup.ButtonsContainer.FocusButtonAtIndex(1);
    }
}