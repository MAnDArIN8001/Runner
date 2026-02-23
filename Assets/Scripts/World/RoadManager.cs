using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] private Material platformMaterial;
    [SerializeField] private float platformSpeed = 10f;
    
    private float gameSpeed;
    
    private void OnEnable()
    {
        GameEvents.OnGameSpeedChanged += UpdatePlatformSpeeds;
    }
    
    private void OnDisable()
    {
        GameEvents.OnGameSpeedChanged -= UpdatePlatformSpeeds;
    }
    
    private void Start()
    {
        gameSpeed = GameManager.Instance.GetCurrentGameSpeed();
    }
    
    private void Update()
    {
        // Update platform speeds based on game speed
        gameSpeed = GameManager.Instance.GetCurrentGameSpeed();
        UpdateAllPlatformSpeeds();
    }
    
    private void UpdatePlatformSpeeds(float newSpeed)
    {
        gameSpeed = newSpeed;
        UpdateAllPlatformSpeeds();
    }
    
    private void UpdateAllPlatformSpeeds()
    {
        Platform[] platforms = FindObjectsOfType<Platform>();
        foreach (Platform platform in platforms)
        {
            // Note: Platform class would need a SetSpeed method to implement this
            // For now, speed is set at initialization
        }
    }
}
