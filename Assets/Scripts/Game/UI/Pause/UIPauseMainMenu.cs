using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class UIPauseMainMenu : UIMenuBaseInteractive
{
    #region Dependencies

    [SerializeField] private GameState gameState;

    [SerializeField] private UIConfirmExitMenu UIConfirmExitMenu;

    #endregion Dependencies

    #region Life Cycle

    protected override void Awake()
    {
        base.Awake();

        //        UIConfirmExit.ExitMenu();

        Assert.IsNotNull(UIConfirmExitMenu);
        UIConfirmExitMenu.ExitMenu();
    }

    #endregion Life Cycle

    #region Methods

    protected override void OnButtonClicked(UIMenuEnum commandId)
    {
        DebugLog($"Command '{commandId}' activated.");
        switch (commandId)
        {
            case UIMenuEnum.Resume:
                ExitMenu();
                break;

            case UIMenuEnum.Options:
                ShowOptionsCanvas();
                break;

            case UIMenuEnum.Exit:
                ShowConfirmExitCanvas();
                break;

            default:
                Assert.IsTrue(false, $"The command {commandId} is not recognised or supported.");
                break;
        }
    }

    private void ShowConfirmExitCanvas()
    {
        HideMenu();
        UIConfirmExitMenu.OnMenuExit += OnUIConfirmExitMenuExited;

        UIConfirmExitMenu.OpenMenu();
    }

    private void OnUIConfirmExitMenuExited()
    {
        UIConfirmExitMenu.OnMenuExit -= OnUIConfirmExitMenuExited;
        ShowMenu();
    }

    private void ShowOptionsCanvas()
    {
        //    throw new NotImplementedException();
    }

    #endregion Methods
}