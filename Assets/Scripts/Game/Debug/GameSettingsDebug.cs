using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameSettingsDebug : BaseBehaviour
{
    [Range(0, 1)]
    public float TimeScale = 1;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == TimeScale)
            return;

        DebugLog($"Updating timescale = {Time.timeScale} -> {TimeScale}");
        Time.timeScale = TimeScale;
    }
}
