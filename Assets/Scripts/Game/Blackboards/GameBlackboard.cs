using UnityEngine;

[RequireComponent(typeof(GameState))]
[RequireComponent(typeof(GameSettings))]
public class GameBlackboard : BaseBlackboard
{
    private GameState _gameState;
    private GameSettings _gameSettings;
    public GameState GameState => GetInitialisedComponent(ref _gameState);

    public GameSettings GameSettings => GetInitialisedComponent(ref _gameSettings);

    public static bool IsGamePaused => Blackboards.Instance.GameBlackboard.GameState.IsPaused;
}