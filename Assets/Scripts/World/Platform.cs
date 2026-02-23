using UnityEngine;

/// <summary>
/// Platform that moves toward the player.
/// Obstacles are spawned on this platform.
/// Platform destroys itself when it goes beyond the player.
/// </summary>
public class Platform : MonoBehaviour
{
    private float platformSpeed = 10f;
    private float despawnDistance = 15f;
    private Vector3 startPosition;
    private PlayerController playerController;
    
    public void Initialize(float speed, float despawnDist, PlayerController player)
    {
        platformSpeed = speed;
        despawnDistance = despawnDist;
        playerController = player;
        startPosition = transform.position;
    }
    
    public void SetSpeed(float newSpeed)
    {
        platformSpeed = newSpeed;
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGameRunning()) return;
        
        // Get current game speed for dynamic adjustment
        platformSpeed = GameManager.Instance.GetCurrentGameSpeed();
        
        // Move platform toward player (negative Z direction)
        transform.Translate(Vector3.back * platformSpeed * Time.deltaTime);
        
        // Despawn if passed the player
        if (playerController != null && transform.position.z < playerController.transform.position.z - despawnDistance)
        {
            Destroy(gameObject);
        }
    }
}
