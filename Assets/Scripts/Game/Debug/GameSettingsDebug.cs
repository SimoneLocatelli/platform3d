using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsDebug : MonoBehaviour
{
    [Range(0, 1)]
    public float TimeScale = 1;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = TimeScale;
    }
}
