using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsDebug : BaseBehaviour
{
    [Range(0, 1)]
    public float TimeScale = 1;

    GameState _gameState;

    GameState GameState => GetInitialisedComponent(ref _gameState);

    // Update is called once per frame
    void Update()
    {
        if (GameState.IsPaused)
            return;

        if (Time.timeScale == TimeScale)
            return;

        DebugLog($"Updating timescale = {Time.timeScale} -> {TimeScale}");
        Time.timeScale = TimeScale;
    }
}
