using UnityEngine;

public class PlayerInputManager : BaseBehaviour
{
    #region Private Fields

    public bool UpPressed => _upPressed;
    public bool UpPressedDown => _upPressedDown;

    public bool DownPressed => _downPressed;
    public bool DownPressedDown => _downPressedDown;

    public bool LeftPressed => _leftPressed;
    public bool LeftPressedDown => _leftPressedDown;

    public bool RightPressed => _rightPressed;
    public bool RightPressedDown => _rightPressedDown;

    public bool JumpPressed => _jumpPressed;
    public bool JumpPressedDown => _jumpPressedDown;

    public bool RunPressed => _runPressed;

    public bool AttackPressedDown => _attackPressedDown;

    #endregion Private Fields

    #region Public Properties
    public Vector3 MovementVector => _movementVector;

    #endregion

    #region Props Key Bindings

    [Header("Movement Keys")]
    public KeyCode Up = KeyCode.W;

    public KeyCode Down = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;

    [Header("Actions")]
    public KeyCode Jump = KeyCode.Space;
    public KeyCode Attack = KeyCode.Mouse0;

    public KeyCode Run = KeyCode.LeftControl;

    #endregion Props Key Bindings

    [Header("Debug")]
    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _movementVector = new Vector3();

    [SerializeField]
    [ReadOnlyProperty]
    bool _attackPressed;

    [SerializeField]
    [ReadOnlyProperty]
    bool _attackPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _upPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _upPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _downPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _downPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _leftPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _leftPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _rightPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _rightPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _jumpPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _jumpPressedDown;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _runPressed;

    [SerializeField]
    [ReadOnlyProperty]
    private bool _runPressedDown;

    public void Update()
    {
        _upPressed = Input.GetKey(Up);
        _upPressedDown = Input.GetKeyDown(Up);

        _downPressed = Input.GetKey(Down);
        _downPressedDown = Input.GetKeyDown(Down);

        _leftPressed = Input.GetKey(Left);
        _leftPressedDown = Input.GetKeyDown(Left);

        _rightPressed = Input.GetKey(Right);
        _rightPressedDown = Input.GetKeyDown(Right);

        _jumpPressed = Input.GetKey(Jump);
        _jumpPressedDown = Input.GetKeyDown(Jump);

        _runPressed = Input.GetKey(Run);
        _runPressedDown = Input.GetKeyDown(Run);

        _attackPressed = Input.GetKey(Attack);
        _attackPressedDown = Input.GetKeyDown(Attack);

        UpdateMovementVector();

        DebugLog("Up button pressed", _upPressedDown);
        DebugLog("Down button pressed", _downPressedDown);
        DebugLog("Right button pressed", _rightPressedDown);
        DebugLog("Left button pressed", _leftPressedDown);
        DebugLog("Jump button pressed", _jumpPressedDown);
        DebugLog("Attack button pressed", _attackPressedDown);
        DebugLog("Run button pressed", _runPressedDown);
    }

    private void UpdateMovementVector()
    {
        _movementVector.x = RightPressed ? 1 :
                           LeftPressed ? -1
                           : 0;

        _movementVector.z = UpPressed ? 1 :
                            DownPressed ? -1
                            : 0;
    }
}