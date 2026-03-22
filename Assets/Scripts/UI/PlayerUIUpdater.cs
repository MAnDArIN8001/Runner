using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD and menu panels for single-scene flow — subscribes to <see cref="GameEvents"/>.
/// </summary>
public class PlayerUIUpdater : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayHudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Gameplay HUD")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI bestScoreHudText;
    [SerializeField] private TextMeshProUGUI bonusStatusText;

    [Header("Main menu")]
    [SerializeField] private TextMeshProUGUI menuBestScoreText;

    [Header("Game over")]
    [SerializeField] private TextMeshProUGUI gameOverFinalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverNewRecordText;

    [Header("Buttons (wired in Awake)")]
    [SerializeField] private Button menuStartButton;
    [SerializeField] private Button menuExitButton;
    [SerializeField] private Button pauseContinueButton;
    [SerializeField] private Button pauseMainMenuButton;
    [SerializeField] private Button gameOverRetryButton;
    [SerializeField] private Button gameOverMainMenuButton;

    private void Awake()
    {
        menuStartButton?.onClick.AddListener(OnMenuStartClicked);
        menuExitButton?.onClick.AddListener(OnMenuExitClicked);
        pauseContinueButton?.onClick.AddListener(OnPauseContinueClicked);
        pauseMainMenuButton?.onClick.AddListener(OnPauseMainMenuClicked);
        gameOverRetryButton?.onClick.AddListener(OnGameOverRetryClicked);
        gameOverMainMenuButton?.onClick.AddListener(OnGameOverMainMenuClicked);
    }

    private void OnDestroy()
    {
        menuStartButton?.onClick.RemoveListener(OnMenuStartClicked);
        menuExitButton?.onClick.RemoveListener(OnMenuExitClicked);
        pauseContinueButton?.onClick.RemoveListener(OnPauseContinueClicked);
        pauseMainMenuButton?.onClick.RemoveListener(OnPauseMainMenuClicked);
        gameOverRetryButton?.onClick.RemoveListener(OnGameOverRetryClicked);
        gameOverMainMenuButton?.onClick.RemoveListener(OnGameOverMainMenuClicked);
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            ApplyFlowState(GameManager.Instance.GetFlowState());
            RefreshHighScore(GameManager.Instance.GetHighScore());
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthUI;
        GameEvents.OnScoreChanged += UpdateScoreUI;
        GameEvents.OnGameSpeedChanged += UpdateSpeedUI;
        GameEvents.OnGameReset += OnGameReset;
        GameEvents.OnGameFlowStateChanged += ApplyFlowState;
        GameEvents.OnHighScoreChanged += RefreshHighScore;
        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnBonusPickedUp += OnBonusPickedUp;
        GameEvents.OnPlayerInvulnerabilityChanged += OnInvulnerabilityChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthUI;
        GameEvents.OnScoreChanged -= UpdateScoreUI;
        GameEvents.OnGameSpeedChanged -= UpdateSpeedUI;
        GameEvents.OnGameReset -= OnGameReset;
        GameEvents.OnGameFlowStateChanged -= ApplyFlowState;
        GameEvents.OnHighScoreChanged -= RefreshHighScore;
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnBonusPickedUp -= OnBonusPickedUp;
        GameEvents.OnPlayerInvulnerabilityChanged -= OnInvulnerabilityChanged;
    }

    private void OnMenuStartClicked() => GameManager.Instance?.StartNewRun();
    private void OnMenuExitClicked() => GameManager.Instance?.QuitGame();
    private void OnPauseContinueClicked() => GameManager.Instance?.ResumeGame();
    private void OnPauseMainMenuClicked() => GameManager.Instance?.ReturnToMainMenu();
    private void OnGameOverRetryClicked() => GameManager.Instance?.StartNewRun();
    private void OnGameOverMainMenuClicked() => GameManager.Instance?.ReturnToMainMenu();

    private void OnGameReset()
    {
        if (gameOverNewRecordText != null)
            gameOverNewRecordText.gameObject.SetActive(false);
        if (bonusStatusText != null)
            bonusStatusText.text = "";
    }

    private void ApplyFlowState(GameFlowState state)
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(state == GameFlowState.MainMenu);
        if (gameplayHudPanel != null)
            gameplayHudPanel.SetActive(state == GameFlowState.Playing || state == GameFlowState.Paused);
        if (pausePanel != null)
            pausePanel.SetActive(state == GameFlowState.Paused);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(state == GameFlowState.GameOver);
    }

    private void RefreshHighScore(int highScore)
    {
        string line = $"Best: {highScore}";
        if (menuBestScoreText != null)
            menuBestScoreText.text = line;
        if (bestScoreHudText != null)
            bestScoreHudText.text = line;
    }

    private void OnGameOver(int finalScore, bool isNewHighScore)
    {
        if (gameOverFinalScoreText != null)
            gameOverFinalScoreText.text = $"Score: {finalScore}";
        if (gameOverNewRecordText != null)
        {
            gameOverNewRecordText.gameObject.SetActive(isNewHighScore);
            if (isNewHighScore)
                gameOverNewRecordText.text = "New best score!";
        }
    }

    private void OnBonusPickedUp(BonusPickupDefinition bonus)
    {
        if (bonus == null || bonusStatusText == null)
            return;
        bonusStatusText.text = bonus.Kind switch
        {
            BonusPickupKind.Heal => $"Heal +{bonus.HealAmount}",
            BonusPickupKind.Invulnerability => $"Shield {bonus.InvulnerabilityDuration:0.#}s",
            _ => "Bonus"
        };
    }

    private void OnInvulnerabilityChanged(bool active, float timeRemaining)
    {
        if (bonusStatusText == null)
            return;
        if (active && timeRemaining > 0.05f)
            bonusStatusText.text = $"Invulnerable: {timeRemaining:0.#}s";
        else if (!active && bonusStatusText != null && bonusStatusText.text.StartsWith("Invulnerable"))
            bonusStatusText.text = "";
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateSpeedUI(float speed)
    {
        if (speedText != null)
            speedText.text = $"Speed: {speed:F1}";
    }
}
