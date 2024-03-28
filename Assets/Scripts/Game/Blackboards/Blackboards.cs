using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CameraBlackboard))]
[RequireComponent(typeof(PlayerBlackboard))]
[RequireComponent(typeof(GameBlackboard))]
[RequireComponent(typeof(AudioBlackboard))]
public class Blackboards : BaseBehaviourLight

{
    [ReadOnlyProperty][SerializeField] private CameraBlackboard _cameraBlackboard;
    [ReadOnlyProperty][SerializeField] private PlayerBlackboard _playerBlackboard;
    [ReadOnlyProperty][SerializeField] private GameBlackboard _gameBlackboard;
    [ReadOnlyProperty][SerializeField] private AudioBlackboard _audioBlackboard;
    
    public CameraBlackboard CameraBlackboard { [DebuggerStepThrough] get => GetInitialisedComponent(ref _cameraBlackboard); }

    public PlayerBlackboard PlayerBlackboard { [DebuggerStepThrough] get => GetInitialisedComponent(ref _playerBlackboard); }

    public GameBlackboard GameBlackboard { [DebuggerStepThrough] get => GetInitialisedComponent(ref _gameBlackboard); }

    public AudioBlackboard AudioBlackboard { [DebuggerStepThrough] get => GetInitialisedComponent(ref _audioBlackboard); }

    private static Blackboards _instance;

    public static Blackboards Instance
    {
        [DebuggerStepThrough]
        get
        {
            if (_instance != null)
                return _instance;

            const string gameManagerObjName = "Game Manager";
            _instance = GameObject.Find(gameManagerObjName).GetComponent<Blackboards>();
            Assert.IsNotNull(_instance, $"Couldn't find the component {nameof(Blackboards)} on the Game Object '{gameManagerObjName}'");
            return _instance;
        }
    }
}