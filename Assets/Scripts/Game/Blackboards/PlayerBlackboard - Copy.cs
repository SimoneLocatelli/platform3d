using UnityEngine;

public class CameraBlackboard : BaseBlackboard
{
    #region Singleton

    private static CameraBlackboard _instance;

    public static CameraBlackboard Instance => _instance ?? (_instance = new CameraBlackboard());

    #endregion Singleton


    public Camera Camera => Camera.main;

    public Transform CameraTransform => Camera.transform;

    private CameraBlackboard()
    {
    }
}