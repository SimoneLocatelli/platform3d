using UnityEngine;

public class CameraBlackboard : BaseBlackboard
{
    public Camera Camera => Camera.main;

    public Transform CameraTransform => Camera.transform;
}