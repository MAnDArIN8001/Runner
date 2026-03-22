using UnityEngine;

/// <summary>High-level single-scene flow for menus, play, pause, and game over.</summary>
public enum GameFlowState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

/// <summary>
/// Centralized event system for the game using delegates.
/// All game events are defined here for event-based architecture.
/// </summary>
public static class GameEvents
{
    // Player events
    public delegate void PlayerMovedDelegate(int laneIndex);
    public static event PlayerMovedDelegate OnPlayerMoved;
    
    public delegate void PlayerJumpedDelegate();
    public static event PlayerJumpedDelegate OnPlayerJumped;
    
    public delegate void PlayerDamagedDelegate(int damage, int remainingHealth);
    public static event PlayerDamagedDelegate OnPlayerDamaged;
    
    public delegate void PlayerDiedDelegate();
    public static event PlayerDiedDelegate OnPlayerDied;
    
    public delegate void PlayerHealthChangedDelegate(int currentHealth, int maxHealth);
    public static event PlayerHealthChangedDelegate OnPlayerHealthChanged;

    public delegate void BonusPickedUpDelegate(BonusPickupDefinition bonus);
    public static event BonusPickedUpDelegate OnBonusPickedUp;

    public delegate void PlayerInvulnerabilityChangedDelegate(bool active, float timeRemaining);
    public static event PlayerInvulnerabilityChangedDelegate OnPlayerInvulnerabilityChanged;
    
    // Game events
    public delegate void GameStartedDelegate();
    public static event GameStartedDelegate OnGameStarted;
    
    public delegate void GamePausedDelegate();
    public static event GamePausedDelegate OnGamePaused;
    
    public delegate void GameResumedDelegate();
    public static event GameResumedDelegate OnGameResumed;
    
    public delegate void GameResetDelegate();
    public static event GameResetDelegate OnGameReset;
    
    // Obstacle events
    public delegate void ObstacleSpawnedDelegate(Obstacle obstacle);
    public static event ObstacleSpawnedDelegate OnObstacleSpawned;
    
    public delegate void ObstacleDestroyedDelegate(Obstacle obstacle);
    public static event ObstacleDestroyedDelegate OnObstacleDestroyed;
    
    // Score/Progression events
    public delegate void ScoreChangedDelegate(int newScore);
    public static event ScoreChangedDelegate OnScoreChanged;
    
    public delegate void GameSpeedChangedDelegate(float newSpeed);
    public static event GameSpeedChangedDelegate OnGameSpeedChanged;

    public delegate void GameFlowStateChangedDelegate(GameFlowState state);
    public static event GameFlowStateChangedDelegate OnGameFlowStateChanged;

    public delegate void HighScoreChangedDelegate(int highScore);
    public static event HighScoreChangedDelegate OnHighScoreChanged;

    public delegate void GameOverDelegate(int finalScore, bool isNewHighScore);
    public static event GameOverDelegate OnGameOver;
    
    // Invoke methods for external callers
    public static void InvokePlayerMoved(int laneIndex) => OnPlayerMoved?.Invoke(laneIndex);
    public static void InvokePlayerJumped() => OnPlayerJumped?.Invoke();
    public static void InvokePlayerDamaged(int damage, int remainingHealth) => OnPlayerDamaged?.Invoke(damage, remainingHealth);
    public static void InvokePlayerDied() => OnPlayerDied?.Invoke();
    public static void InvokePlayerHealthChanged(int currentHealth, int maxHealth) => OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    public static void InvokeBonusPickedUp(BonusPickupDefinition bonus) => OnBonusPickedUp?.Invoke(bonus);
    public static void InvokePlayerInvulnerabilityChanged(bool active, float timeRemaining) =>
        OnPlayerInvulnerabilityChanged?.Invoke(active, timeRemaining);
    public static void InvokeGameStarted() => OnGameStarted?.Invoke();
    public static void InvokeGamePaused() => OnGamePaused?.Invoke();
    public static void InvokeGameResumed() => OnGameResumed?.Invoke();
    public static void InvokeGameReset() => OnGameReset?.Invoke();
    public static void InvokeObstacleSpawned(Obstacle obstacle) => OnObstacleSpawned?.Invoke(obstacle);
    public static void InvokeObstacleDestroyed(Obstacle obstacle) => OnObstacleDestroyed?.Invoke(obstacle);
    public static void InvokeScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public static void InvokeGameSpeedChanged(float newSpeed) => OnGameSpeedChanged?.Invoke(newSpeed);
    public static void InvokeGameFlowStateChanged(GameFlowState state) => OnGameFlowStateChanged?.Invoke(state);
    public static void InvokeHighScoreChanged(int highScore) => OnHighScoreChanged?.Invoke(highScore);
    public static void InvokeGameOver(int finalScore, bool isNewHighScore) => OnGameOver?.Invoke(finalScore, isNewHighScore);
}
