using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(Canvas))]
public abstract class UIMenuBase : BaseBehaviourLight
{
    #region Events

    public OnMenuExitEventHandler OnMenuExit;

    public OnMenuOpenEventHandler OnMenuOpen;

    public delegate void OnMenuExitEventHandler();

    public delegate void OnMenuOpenEventHandler();

    #endregion Events

    #region Props Info

    [Header("Info")]
    [SerializeField][ReadOnlyProperty] private bool isMenuActive = false;

    #endregion Props Info

    #region Dependencies

    [SerializeField] private AudioManager _audioManager;

    private Canvas _canvas;

    [SerializeField][ReadOnlyProperty] private PlayerInputManager _playerInputManager;

    protected AudioManager AudioManager => GetInitialisedComponent(ref _audioManager);
    protected Canvas Canvas => GetInitialisedComponent(ref _canvas);
    protected PlayerInputManager PlayerInputManager => _playerInputManager;

    #endregion Dependencies

    #region Properties

    public bool IsCanvasEnabled => Canvas.enabled;
    public bool IsMenuActive => isMenuActive;
    protected string TypeName { get; private set; }

    #endregion Properties

    #region Fields

    private bool firstUpdate;

    #endregion Fields

    #region Life Cycle

    protected virtual void Awake()
    {
        TypeName = GetType().Name;
        _playerInputManager = _playerInputManager != null ? _playerInputManager : Blackboards.Instance.PlayerBlackboard.PlayerInputManager;
    }

    protected virtual void Update()
    {
        if (!isMenuActive)
            return;

        if (firstUpdate)
        {
            firstUpdate = false;
            return;
        }

        HandleMenu();
    }

    #endregion Life Cycle

    #region Methods

    public virtual void ExitMenu()
    {
        DebugLog($"Menu '{TypeName}' - Exiting menu");
        HideMenu();
        OnMenuExit?.Invoke();
    }

    public virtual void OpenMenu()
    {
        DebugLog($"Menu '{TypeName}' - Opening menu");
        ShowMenu();
        OnMenuOpen?.Invoke();
    }

    protected virtual bool HandleMenu()
    {
        if (_playerInputManager.PausePressedDown)
            ExitMenu();
        else
            return false;

        return true;
    }

    protected void HideMenu()
    {
        isMenuActive = false;
        Canvas.enabled = false;
    }

    protected void ShowMenu()
    {
        isMenuActive = true;
        firstUpdate = true;
        Canvas.enabled = true;
    }

    #endregion Methods
}