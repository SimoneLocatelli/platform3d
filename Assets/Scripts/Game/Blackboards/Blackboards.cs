using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CameraBlackboard))]
[RequireComponent(typeof(PlayerBlackboard))]
public class Blackboards : BaseBehaviourLight

{
    [ReadOnlyProperty][SerializeField] private CameraBlackboard _cameraBlackboard;
    [ReadOnlyProperty][SerializeField] private PlayerBlackboard _playerBlackboard;

    public CameraBlackboard CameraBlackboard => GetInitialisedComponent(ref _cameraBlackboard);

    public PlayerBlackboard PlayerBlackboard => GetInitialisedComponent(ref _playerBlackboard);

    private static Blackboards _instance;

    public static Blackboards Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            var blackboards = GameObject.Find("Game Manager").GetComponent<Blackboards>();
            Assert.IsNotNull(blackboards);
            return blackboards;
        }
    }
}