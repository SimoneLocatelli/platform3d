using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool IsPaused;

    private float prePauseTimescale;

    public void StartPause()
    {
        IsPaused = true;
        prePauseTimescale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void StopPause()
    {
        IsPaused = false;
        Time.timeScale = prePauseTimescale;
    }
}