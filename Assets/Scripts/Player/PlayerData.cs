using UnityEngine;

/// <summary>
/// Player data layer - manages health and state.
/// Pure data container with health logic.
/// </summary>
public class PlayerData : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    
    private int currentHealth;
    private int maxHealth;
    private bool isAlive;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => isAlive;
    
    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("PlayerConfig not assigned in PlayerData!");
            return;
        }
        
        maxHealth = config.maxHealth;
        currentHealth = config.defaultStartHealth;
        isAlive = true;
        
        GameEvents.InvokePlayerHealthChanged(currentHealth, maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        GameEvents.InvokePlayerDamaged(damage, currentHealth);
        GameEvents.InvokePlayerHealthChanged(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            isAlive = false;
            GameEvents.InvokePlayerDied();
        }
    }
    
    public void Heal(int amount)
    {
        if (!isAlive) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        GameEvents.InvokePlayerHealthChanged(currentHealth, maxHealth);
    }
    
    public void ResetHealth()
    {
        currentHealth = config.defaultStartHealth;
        isAlive = true;
        GameEvents.InvokePlayerHealthChanged(currentHealth, maxHealth);
    }
}
