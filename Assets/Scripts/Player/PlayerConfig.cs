using UnityEngine;

/// <summary>
/// ScriptableObject configuration for player stats and movement.
/// Stores static data to avoid duplication in memory.
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Runner/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] public float forwardSpeed = 15f;
    [SerializeField] public float laneChangeSpeed = 50f;  // Increased for more reactive response
    [SerializeField] public float laneWidth = 2f;
    
    [Header("Jumping")]
    [SerializeField] public float jumpHeight = 3f;  // Maximum jump height (fixed)
    [SerializeField] public float jumpLengthZ = 5f;  // Fixed horizontal distance in jump (independent of speed)
    [SerializeField] public float jumpDuration = 0.6f;  // Duration of jump animation
    // The drag fields were used by the Rigidbody-based controller. They are
    // retained for backwards compatibility but ignored when using
    // CharacterController.
    [SerializeField] public float groundDrag = 5f;
    [SerializeField] public float airDrag = 0.5f;
    
    [Header("Lane Swapping")]
    [SerializeField] public float laneSwapDuration = 0.3f;  // Time to swap lanes (constant, configurable)
    
    [Header("Health")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int defaultStartHealth = 100;
    
    [Header("Detection")]
    [SerializeField] public float raycastDistance = 0.5f;
}
