using UnityEngine;

public enum BonusPickupKind
{
    Heal,
    Invulnerability
}

/// <summary>
/// Data-driven definition for a bonus pickup (heal amount, invulnerability duration, etc.).
/// </summary>
[CreateAssetMenu(fileName = "BonusPickup", menuName = "Runner/Bonus Pickup Definition")]
public class BonusPickupDefinition : ScriptableObject
{
    [SerializeField] private BonusPickupKind kind = BonusPickupKind.Heal;
    [SerializeField] private int healAmount = 30;
    [SerializeField] private float invulnerabilityDuration = 5f;

    public BonusPickupKind Kind => kind;
    public int HealAmount => healAmount;
    public float InvulnerabilityDuration => invulnerabilityDuration;
}
