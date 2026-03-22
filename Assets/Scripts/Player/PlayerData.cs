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
    private float invulnerabilityTimeRemaining;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => isAlive;
    public bool IsInvulnerable => invulnerabilityTimeRemaining > 0f;
    public float InvulnerabilityTimeRemaining => invulnerabilityTimeRemaining;
    
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

    private void Update()
    {
        if (invulnerabilityTimeRemaining <= 0f)
            return;

        invulnerabilityTimeRemaining -= Time.deltaTime;
        if (invulnerabilityTimeRemaining <= 0f)
        {
            invulnerabilityTimeRemaining = 0f;
            GameEvents.InvokePlayerInvulnerabilityChanged(false, 0f);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        if (IsInvulnerable) return;

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

    public void ApplyBonus(BonusPickupDefinition def)
    {
        if (def == null || !isAlive)
            return;

        switch (def.Kind)
        {
            case BonusPickupKind.Heal:
                Heal(def.HealAmount);
                break;
            case BonusPickupKind.Invulnerability:
                GrantInvulnerability(def.InvulnerabilityDuration);
                break;
        }
    }

    private void GrantInvulnerability(float duration)
    {
        if (duration <= 0f)
            return;

        invulnerabilityTimeRemaining = Mathf.Max(invulnerabilityTimeRemaining, duration);
        GameEvents.InvokePlayerInvulnerabilityChanged(true, invulnerabilityTimeRemaining);
    }
    
    public void ResetHealth()
    {
        bool hadInvulnerability = invulnerabilityTimeRemaining > 0f;
        invulnerabilityTimeRemaining = 0f;
        currentHealth = config.defaultStartHealth;
        isAlive = true;
        GameEvents.InvokePlayerHealthChanged(currentHealth, maxHealth);
        if (hadInvulnerability)
            GameEvents.InvokePlayerInvulnerabilityChanged(false, 0f);
    }
}
