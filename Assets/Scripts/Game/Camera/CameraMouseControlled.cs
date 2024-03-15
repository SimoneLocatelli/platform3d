using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMouseControlled : BaseBehaviour
{
    #region Private Fields

    private readonly MouseSystem mouseSystem = new MouseSystem();

    #endregion Private Fields

    #region Props Settings

    [Header("Settings")]
    [Range(1, 10f)]
    [SerializeField]
    private float radius = 1;

    [Range(1, 20)]
    [SerializeField]
    private float sensitivity = 20;

    [SerializeField]
    private bool forceRefresh;

    [SerializeField]
    private Transform target;

    [Range(0.5f, 5f)]
    private float zoomScale = 1f;

    #endregion Props Settings

    #region Props Settings

    [Header("Debug")]
    [SerializeField]
    [ReadOnlyProperty]
    private Vector2 mouseAxis;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isInsideGameScreen;

    [SerializeField]
    [ReadOnlyProperty]
    private float Angle = 1;

    #endregion Props Settings

    #region Lifecycle

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        UpdateCameraZoom();
        UpdateCameraPosition();
    }

    #endregion Lifecycle

    #region Methods

    private void UpdateCameraPosition()
    {
        mouseAxis = mouseSystem.Axis;

        if (mouseSystem.IsInsideGameScreen() && mouseAxis.x != 0)
        {
            var newAngle = Angle - (mouseAxis.x * sensitivity * Time.deltaTime);
            var delta = newAngle - Angle;
            delta = Mathf.Clamp(delta, -0.1f, 0.1f);
            Angle += delta;
        }

        float x = Mathf.Cos(Angle) * radius;
        float z = Mathf.Sin(Angle) * radius;
        transform.position = new Vector3(x, 0, z) + target.position;

        transform.LookAt(target);
    }

    private void UpdateCameraZoom()
    {
        if (!mouseSystem.IsInsideGameScreen())
            return;

        if (mouseSystem.IsZooming(out Vector2 mouseScrollDelta))
            radius -= mouseScrollDelta.y * zoomScale;
    }

    #endregion Methods
}