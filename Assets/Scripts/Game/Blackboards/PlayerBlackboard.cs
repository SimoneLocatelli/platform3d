using UnityEngine;

public class PlayerBlackboard : BaseBlackboard
{
    #region Singleton

    private static PlayerBlackboard _instance;

    public static PlayerBlackboard Instance => _instance ?? (_instance = new PlayerBlackboard());

    #endregion Singleton

    private PlayerController _playerController;

    public PlayerController PlayerController => FindObjectOfType(ref _playerController);

    public Transform PlayerTransform => PlayerController.transform;

    private PlayerBlackboard()
    {
    }
}