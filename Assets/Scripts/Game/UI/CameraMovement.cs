using System;
using UnityEngine;

public class CameraMovement : BaseBehaviour
{
    #region Properties

    [Header("Camera Settings")]
    [Range(10, 25)]
    public int CameraSpeed = 5;

    [Range(0.1f, 3)]
    public float CameraZoomSpeed = 1;

    #endregion Properties

    #region Enums

    public enum CameraDirections
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }


    #endregion
 
    public void MoveCamera(CameraDirections direction)
    {
        if (direction == CameraDirections.None)
            return;

        var cameraSpeedNormalised = Time.deltaTime * CameraSpeed;

        var x = direction.HasFlag(CameraDirections.Right) ? cameraSpeedNormalised :
            direction.HasFlag(CameraDirections.Left) ? -cameraSpeedNormalised : 0;

        var y = direction.HasFlag(CameraDirections.Up) ? cameraSpeedNormalised :
                direction.HasFlag(CameraDirections.Down) ? -cameraSpeedNormalised : 0;

        Camera.main.transform.Translate(new Vector3(x, y));
    }

    public void UpdateZoom(float delta)
    {
        var newSize = Camera.main.orthographicSize + (delta * -CameraZoomSpeed);

        Camera.main.orthographicSize = Math.Min(25, Math.Max(5, newSize));
    }
}