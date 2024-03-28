using UnityEngine;

public class PlayerBlackboard : BaseBlackboard
{
    #region Props - Readonly

    [ReadOnlyProperty][SerializeField] private PlayerController _playerController;
    [ReadOnlyProperty][SerializeField] private GameObject _playerGameObject;
    [ReadOnlyProperty][SerializeField] private PlayerInputManager _playerInputManager;
    [ReadOnlyProperty][SerializeField] private LifeSystem _playerLifeSystem;

    #endregion Props - Readonly

    #region Props

    [SerializeField] private LayerMask _playerLayerMask;

    #endregion Props

    public LifeSystem LifeSystem
        => GetInitialisedComponent(PlayerGameObject, ref _playerLifeSystem);

    public PlayerController PlayerController
    {
        get
        {
            if (_playerController == null)
                _playerController = FindObjectOfType(ref _playerController);
            return _playerController;
        }
    }

    public GameObject PlayerGameObject
    {
        get
        {
            if (_playerGameObject == null)
                _playerGameObject = PlayerController.gameObject;
            return _playerGameObject;
        }
    }
    public PlayerInputManager PlayerInputManager
        => GetInitialisedComponent(PlayerGameObject, ref _playerInputManager);

    public LayerMask PlayerLayerMask => _playerLayerMask;

    public Vector3 PlayerPosition => PlayerTransform.position;

    public Transform PlayerTransform => PlayerController.transform;

    public float StaminaPercentage => PlayerController.StaminaPercentage;
}