using UnityEngine;

/// <summary>
/// Main game manager — single-scene flow (main menu, playing, paused, game over), speed, score, high score.
/// </summary>
[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float baseGameSpeed = 10f;
    [SerializeField] private float speedIncreasePerSecond = 0.5f;
    [SerializeField] private bool enableSpeedAcceleration = true;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private float currentGameSpeed;
    private float elapsedTime;
    private bool isGameRunning;
    private bool isGamePaused;
    private GameFlowState flowState = GameFlowState.MainMenu;
    private int highScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (obstacleSpawner == null)
            obstacleSpawner = FindFirstObjectByType<ObstacleSpawner>();

        highScore = HighScoreStorage.Load();
    }

    private void Start()
    {
        GameEvents.InvokeHighScoreChanged(highScore);
        EnterMainMenu(resetWorld: false);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += HandlePlayerDead;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= HandlePlayerDead;
    }

    private void Update()
    {
        if (!isGameRunning || isGamePaused)
            return;

        elapsedTime += Time.deltaTime;

        if (enableSpeedAcceleration)
        {
            currentGameSpeed = baseGameSpeed + elapsedTime * speedIncreasePerSecond;
            GameEvents.InvokeGameSpeedChanged(currentGameSpeed);
        }

        int score = Mathf.FloorToInt(elapsedTime);
        GameEvents.InvokeScoreChanged(score);
    }

    /// <summary>Called from Input System (e.g. ESC) — cannot use UnityEngine.Input when active input is Input System only.</summary>
    public void TryTogglePauseFromInput()
    {
        if (flowState == GameFlowState.Playing)
            PauseGame();
        else if (flowState == GameFlowState.Paused)
            ResumeGame();
    }

    /// <summary>Initial menu after load — does not rebuild the world (spawner already generated a start layout).</summary>
    public void EnterMainMenu(bool resetWorld)
    {
        CancelInvoke();
        Time.timeScale = 1f;
        isGamePaused = false;
        isGameRunning = false;
        elapsedTime = 0f;
        currentGameSpeed = baseGameSpeed;

        if (resetWorld)
        {
            GameEvents.InvokeGameReset();
            obstacleSpawner?.ResetAndRegenerateWorld();
        }

        SetFlowState(GameFlowState.MainMenu);
        GameEvents.InvokeGameSpeedChanged(currentGameSpeed);
        GameEvents.InvokeScoreChanged(0);
    }

    /// <summary>Start a new run from the main menu or from the game-over screen.</summary>
    public void StartNewRun()
    {
        if (flowState != GameFlowState.MainMenu && flowState != GameFlowState.GameOver)
            return;

        CancelInvoke();
        Time.timeScale = 1f;
        isGamePaused = false;

        GameEvents.InvokeGameReset();
        obstacleSpawner?.ResetAndRegenerateWorld();

        elapsedTime = 0f;
        currentGameSpeed = baseGameSpeed;
        isGameRunning = true;

        SetFlowState(GameFlowState.Playing);
        GameEvents.InvokeGameStarted();
        GameEvents.InvokeGameSpeedChanged(currentGameSpeed);
        GameEvents.InvokeScoreChanged(0);
    }

    private void HandlePlayerDead()
    {
        if (!isGameRunning)
            return;

        CancelInvoke();
        isGameRunning = false;
        isGamePaused = false;
        Time.timeScale = 1f;

        int finalScore = Mathf.FloorToInt(elapsedTime);
        bool isNewHigh = finalScore > highScore;
        if (isNewHigh)
        {
            highScore = finalScore;
            HighScoreStorage.Save(highScore);
            GameEvents.InvokeHighScoreChanged(highScore);
        }

        SetFlowState(GameFlowState.GameOver);
        GameEvents.InvokeGameOver(finalScore, isNewHigh);
        Debug.Log($"Game Over! Final Score: {finalScore}");
    }

    public void ReturnToMainMenu()
    {
        if (flowState != GameFlowState.Paused && flowState != GameFlowState.GameOver)
            return;

        EnterMainMenu(resetWorld: true);
    }

    public void PauseGame()
    {
        if (flowState != GameFlowState.Playing)
            return;

        isGamePaused = true;
        Time.timeScale = 0f;
        SetFlowState(GameFlowState.Paused);
        GameEvents.InvokeGamePaused();
    }

    public void ResumeGame()
    {
        if (flowState != GameFlowState.Paused)
            return;

        isGamePaused = false;
        Time.timeScale = 1f;
        SetFlowState(GameFlowState.Playing);
        GameEvents.InvokeGameResumed();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public float GetCurrentGameSpeed() => currentGameSpeed;
    public bool IsGameRunning() => isGameRunning && !isGamePaused;
    public GameFlowState GetFlowState() => flowState;
    public int GetHighScore() => highScore;
    public int GetCurrentScore() => Mathf.FloorToInt(elapsedTime);

    private void SetFlowState(GameFlowState state)
    {
        flowState = state;
        GameEvents.InvokeGameFlowStateChanged(state);
    }
}
