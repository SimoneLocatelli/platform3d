using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class UIConfirmExitMenu : UIMenuBaseInteractive
{
    #region Methods

    protected override bool HandleMenu()
    {
        if (base.HandleMenu())
            return true;

        if (PlayerInputManager.LeftPressedDown || PlayerInputManager.RightPressedDown)
            MoveCursorUp();
        else
            return false;

        return true;
    }

    protected override void OnButtonClicked(UIMenuEnum commandId)
    {
        DebugLog($"Command '{commandId}' activated.");
        switch (commandId)
        {
            case UIMenuEnum.Yes:
                ExitGame();
                break;

            case UIMenuEnum.No:
                ExitMenu();
                break;

            default:
                Assert.IsTrue(false, $"The command {commandId} is not recognised.");
                break;
        }
    }

    private void ExitGame()
    {
        DebugLog("Exiting game");
        ApplicationTerminator.ExitGame();
    }

    #endregion Methods
}