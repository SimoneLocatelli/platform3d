using System;
using UnityEngine;
using UnityEngine.Assertions;
using static CameraMovement;

public class Cursor : BaseBehaviour
{
    #region Properties

    public CameraMovement CameraMovement;

    [Header("Cursor Sprites")]
    public Texture2D Crosshair;

    public Texture2D CrosshairDown;

    [Space]
    [Header("Camera Move Area")]
    [Range(100, 200)]
    public int CameraMoveSizeX;

    [Range(100, 200)]
    public int CameraMoveSizeY;

    [Space]
    [Header("Debug")]
    [ReadOnlyProperty]
    public int Down;

    [ReadOnlyProperty]
    public int Left;

    [ReadOnlyProperty]
    public int Right;

    [ReadOnlyProperty]
    public int Up;

    private readonly MouseSystem mouseSystem = new MouseSystem();
    private Vector2 crosshairDownOffset;
    private Vector2 crosshairOffset;
    private CursorStates CursorState = CursorStates.Default;

    #endregion Properties

    #region Enums

    private enum CursorStates
    {
        Default,
        Down
    }

    #endregion Enums

    public void Update()
    {
        Assert.IsNotNull(CameraMovement);

        UpdateMouseIcon();

        if (mouseSystem.IsZooming(out Vector2 mouseScrollDelta))
            CameraMovement.UpdateZoom(mouseScrollDelta.y);

        var cameraDirection = GetCursorCameraMovement();
        CameraMovement.MoveCamera(cameraDirection);
    }

    private static void UpdateCursor(Texture2D crossHairTexture, Vector2 cursorOffset)
    {
        UnityEngine.Cursor.SetCursor(crossHairTexture, cursorOffset, CursorMode.Auto);
    }

    private CameraDirections GetCursorCameraMovement()
    {
        var direction = CameraDirections.None;

        if (!mouseSystem.IsInsideGameScreen(out Vector2 mousePosition))
            return direction;

        //DebugLog(mouseSystem.GetMousePosition());

        Left = CameraMoveSizeX;
        Down = CameraMoveSizeY;
        Right = Screen.width - CameraMoveSizeX;
        Up = Screen.height - CameraMoveSizeY;

        if (mousePosition.x < Left)
            direction |= CameraDirections.Left;
        if (mousePosition.x > Right)
            direction |= CameraDirections.Right;
        if (mousePosition.y < Down)
            direction |= CameraDirections.Down;
        if (mousePosition.y > Up)
            direction |= CameraDirections.Up;

        if (direction != CameraDirections.None)
            DebugLog($"Moving camera: {direction}");

        return direction;
    }

    private void Start()
    {
        crosshairOffset = new Vector2(Crosshair.width / 2, Crosshair.height / 2);
        crosshairDownOffset = new Vector2(CrosshairDown.width / 2, CrosshairDown.height / 2);
        UnityEngine.Cursor.visible = true;
        UpdateCursor(Crosshair, crosshairOffset);
    }

    private void UpdateMouseIcon()
    {
        if (mouseSystem.HasPressedLeftButton())
        {
            if (CursorState == CursorStates.Default)
            {
                UpdateCursor(CrosshairDown, crosshairDownOffset);
                CursorState = CursorStates.Down;
            }
        }
        else if (mouseSystem.HasReleasedLeftButton())
        {
            if (CursorState == CursorStates.Down)
            {
                UpdateCursor(Crosshair, crosshairOffset);
                CursorState = CursorStates.Default;
            }
        }
    }
}