using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class CameraMouseControlled : BaseBehaviour
{
    #region Private Fields

    private readonly MouseSystem mouseSystem = new MouseSystem();

    #endregion Private Fields

    #region Props

    [Header("Tracked Object")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private bool forceRefresh;

    [Header("Mouse Movement")]
    [Range(1, 20)]
    [SerializeField] private float mouseMovementXSensitivity = 20;

    [Header("Zoom")]
    [Range(1, 30)]
    [SerializeField] private int zoom = 1;

    [Range(1, 10)]
    [SerializeField] private float zoomSmoothSpeed = 5f;

    [Range(1, 5)]
    [SerializeField] private int zoomMouseWheelStep = 1;

    [MinMaxRange(1, 30)]
    [SerializeField] private Vector2Int zoomRange;

    [MinMaxRange(1, 10)]
    [SerializeField] private Vector2Int zoomYRange;

    #endregion Props

    #region Props Settings

    [Header("Debug")]
    [SerializeField]
    [Range(-10, 10)]
    private float Angle = 1;

    [SerializeField]
    [ReadOnlyProperty]
    private float radius = 1;

    [SerializeField]
    [ReadOnlyProperty]
    private float zoomFactor = 0;

    [SerializeField]
    [ReadOnlyProperty]
    private float targetY = 0;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isMouseInsideGameScreen;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector2 mouseAxis;

    #endregion Props Settings

    #region Lifecycle

    private void LateUpdate()
    {
        isMouseInsideGameScreen = mouseSystem.IsInsideGameScreen();

        UpdateCameraZoom();
        UpdateCameraPosition();
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (target == null)
                target = Blackboards.Instance.PlayerBlackboard.PlayerTransform;
        }
    }

    #endregion Lifecycle

    #region Methods

    private void UpdateCameraPosition()
    {
        if (!Application.isPlaying && target == null)
            return;

        if (!Application.isPlaying && !forceRefresh)
            return;

        Assert.IsNotNull(target);

        mouseAxis = mouseSystem.Axis;

        if (forceRefresh || (isMouseInsideGameScreen && mouseAxis.x != 0))
        {
            var newAngle = Angle - (mouseAxis.x * mouseMovementXSensitivity * Time.deltaTime);
            var delta = newAngle - Angle;
            delta = Mathf.Clamp(delta, -0.1f, 0.1f);
            Angle += delta;
        }

        float x = Mathf.Cos(Angle) * radius;
        float z = Mathf.Sin(Angle) * radius;

        var targetposition = target.position.Update(y: 0);
        transform.position = new Vector3(x, transform.position.y, z) + targetposition;
        transform.LookAt(target);
    }

    private void UpdateCameraZoom()
    {
        // Zoom in / Zoom out
        bool canZoomInOrOut = isMouseInsideGameScreen && Application.isPlaying;
        if (canZoomInOrOut && mouseSystem.IsZooming(out Vector2 mouseScrollDelta))
            zoom -= Mathf.RoundToInt(mouseScrollDelta.y * zoomMouseWheelStep);

        // Can update view
        if (forceRefresh || Application.isPlaying)
        {
            zoom = Mathf.Clamp(zoom, zoomRange.x, zoomRange.y);
            radius = Application.isPlaying ? Mathf.Lerp(radius, zoom, Time.deltaTime * zoomSmoothSpeed)
                                           : zoom;

            // Normalized zoom factor [0, 1]
            zoomFactor = ((float)zoom - zoomRange.x) / (zoomRange.y - zoomRange.x);

            // From normalised value (zoomFactor [0,1]) to value between min and max Y values;
            targetY = zoomYRange.y * zoomFactor + zoomYRange.x;

            // Smooth Y movement and update position
            var y = Application.isPlaying ? Mathf.Lerp(transform.position.y, targetY, zoomSmoothSpeed * Time.deltaTime)
                                          : targetY;
            transform.position = transform.position.Update(y: y);
        }
    }

    private void Reset()
    {
        if (target == null)
            target = Blackboards.Instance.PlayerBlackboard.PlayerTransform;
    }

    #endregion Methods
}