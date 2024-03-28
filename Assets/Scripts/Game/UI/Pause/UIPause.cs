using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(AudioManager))]
public class UIPause : UIMenuBase
{
    #region Props Info

    [Header("Info")]
    private float prePauseTimescale = 1f;

    #endregion Props Info

    #region Dependencies

    private GameState gameState;

    [Header("Menus")]
    [SerializeField] private UIPauseMainMenu pauseMainMenu;

    #endregion Dependencies


    #region Fields


    private bool needToHandleExit;
    #endregion
    #region Life Cycle


    protected override void Awake()
    {
        base.Awake();

        gameState = gameState != null ? gameState : FindObjectOfType<GameState>();
        pauseMainMenu = pauseMainMenu != null ? pauseMainMenu : gameObject.GetComponentInChildren<UIPauseMainMenu>();

        Assert.IsNotNull(gameState);
        Assert.IsNotNull(pauseMainMenu);

        pauseMainMenu.OnMenuExit = StopPause;
    }

    protected override void Update()
    {
        if (needToHandleExit)
        {

            DebugLog("Exiting pause");
            ExitMenu();
            gameState.IsPaused = false;
            Time.timeScale = prePauseTimescale;
            needToHandleExit = false;
            return;
        }

        if (gameState.IsPaused)
            return;

        if (PlayerInputManager.PausePressedDown)
            StartPause();

    }
    #endregion Life Cycle

    #region Methods

    private void StartPause()
    {
        DebugLog("Starting pause");

        var images = gameObject.GetComponentsInChildren<Image>();

        DoCanvasHack(images);

        pauseMainMenu.OpenMenu();
        OpenMenu();
        gameState.IsPaused = true;
        prePauseTimescale = Time.timeScale;
        Time.timeScale = 0;

    }

    /// <summary>
    /// This method does a weird but necessary hack.
    /// When activating the canvas the first time, various background images appeared to not be loading
    /// despite having the correct image set and being enabled.
    /// It seems that by toggling the images on and off, the issue goes away.
    /// Not sure if it depends on the Unity version or by how I've implemented the UI.
    /// I might open a post on the Unity forum later down the line with Unity.
    /// </summary>
    /// <param name="images"></param>
    private static void DoCanvasHack(Image[] images)
    {
        foreach (var img in images)
            img.enabled = !img.enabled;


        foreach (var img in images)
            img.enabled = !img.enabled;
    }

    private void StopPause()
        => needToHandleExit = true;


    #endregion Methods
}