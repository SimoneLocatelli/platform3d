using System;
using UnityEngine;

public class PlayerBlackboard : BaseBlackboard
{
    [ReadOnlyProperty][SerializeField] private GameObject _playerGameObject;
    [ReadOnlyProperty][SerializeField] private PlayerController _playerController;
    [ReadOnlyProperty][SerializeField] private LifeSystem _playerLifeSystem;

    public PlayerController PlayerController
    {
        get
        {
            if (_playerController != null)
                return _playerController;
            return (_playerController = FindObjectOfType(ref _playerController));
        }
    }

    public Transform PlayerTransform => PlayerController.transform;

    public GameObject PlayerGameObject
    {
        get
        {
            if (_playerGameObject != null)
                return _playerGameObject;
            return (_playerGameObject = PlayerController.gameObject);
        }
    }

    public float StaminaPercentage => PlayerController.StaminaPercentage;

    public LifeSystem LifeSystem
        => GetInitialisedComponent(PlayerGameObject, ref _playerLifeSystem);
}