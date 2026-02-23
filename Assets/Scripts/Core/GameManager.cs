using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game manager - controls game flow, pause, restart, and game speed.
/// Entry point for the game logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private float baseGameSpeed = 10f;
    [SerializeField] private float speedIncreasePerSecond = 0.5f;
    [SerializeField] private bool enableSpeedAcceleration = true;
    
    private float currentGameSpeed;
    private float elapsedTime;
    private bool isGameRunning;
    private bool isGamePaused;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        InitializeGame();
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
        if (!isGameRunning || isGamePaused) return;
        
        elapsedTime += Time.deltaTime;
        
        // Gradually increase game speed
        if (enableSpeedAcceleration)
        {
            currentGameSpeed = baseGameSpeed + (elapsedTime * speedIncreasePerSecond);
            GameEvents.InvokeGameSpeedChanged(currentGameSpeed);
        }
        
        // Simple score based on time survived
        int score = Mathf.FloorToInt(elapsedTime);
        GameEvents.InvokeScoreChanged(score);
    }
    
    private void InitializeGame()
    {
        isGameRunning = false;
        isGamePaused = false;
        currentGameSpeed = baseGameSpeed;
        elapsedTime = 0f;
        GameEvents.InvokeGameReset();
        GameEvents.InvokeGameStarted();
        isGameRunning = true;
    }
    
    private void HandlePlayerDead()
    {
        isGameRunning = false;
        Debug.Log($"Game Over! Final Score: {Mathf.FloorToInt(elapsedTime)}");
        
        // Auto restart after a delay
        Invoke(nameof(RestartGame), 2f);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void PauseGame()
    {
        if (isGameRunning && !isGamePaused)
        {
            isGamePaused = true;
            Time.timeScale = 0f;
            GameEvents.InvokeGamePaused();
        }
    }
    
    public void ResumeGame()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            Time.timeScale = 1f;
            GameEvents.InvokeGameResumed();
        }
    }
    
    public float GetCurrentGameSpeed() => currentGameSpeed;
    public bool IsGameRunning() => isGameRunning && !isGamePaused;
}
