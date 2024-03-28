using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPauseHandler : BaseBehaviourLight
{
    private PlayerInputManager _playerInputManager;

    public bool CanStartPause => !GameState.IsPaused && PlayerInputManager.PausePressedDown;

    private GameState GameState => Blackboards.Instance.GameBlackboard.GameState;
    private PlayerInputManager PlayerInputManager => GetInitialisedComponent<PlayerInputManager>(ref _playerInputManager);
    private void StartPause()
    {
        Blackboards.Instance.GameBlackboard.GameState.StartPause();
        CustomResources.InstantiatePrefab(GameResources.Prefabs.UIToolkit.UIPause);
    }

    private void Update()
    {
        if (CanStartPause)
        {
            StartPause();
        }
    }
}