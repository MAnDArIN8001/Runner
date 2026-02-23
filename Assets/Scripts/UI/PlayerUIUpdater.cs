using UnityEngine;
using TMPro;

/// <summary>
/// Player UI update layer - listens to game events and updates UI.
/// Pure UI update logic separated from game logic.
/// </summary>
public class PlayerUIUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI gameOverPanel;
    
    private void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthUI;
        GameEvents.OnScoreChanged += UpdateScoreUI;
        GameEvents.OnGameSpeedChanged += UpdateSpeedUI;
        GameEvents.OnPlayerDied += ShowGameOverUI;
        GameEvents.OnGameReset += HideGameOverUI;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthUI;
        GameEvents.OnScoreChanged -= UpdateScoreUI;
        GameEvents.OnGameSpeedChanged -= UpdateSpeedUI;
        GameEvents.OnPlayerDied -= ShowGameOverUI;
        GameEvents.OnGameReset -= HideGameOverUI;
    }
    
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
    
    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateSpeedUI(float speed)
    {
        if (speedText != null)
        {
            speedText.text = $"Speed: {speed:F1}";
        }
    }
    
    private void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
        }
    }
    
    private void HideGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(false);
        }
    }
}
