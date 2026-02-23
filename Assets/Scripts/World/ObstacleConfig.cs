using UnityEngine;

/// <summary>
/// Configuration for obstacles and platform generation.
/// Stores platform prefabs and obstacle spawn rules.
/// </summary>
[CreateAssetMenu(fileName = "ObstacleConfig", menuName = "Runner/Obstacle Config")]
public class ObstacleConfig : ScriptableObject
{
    [System.Serializable]
    public class ObstacleType
    {
        public string typeName;
        public GameObject prefab;
        public int damage = 10;
    }
    
    [Header("Platform Prefabs")]
    [SerializeField] public GameObject[] platformPrefabs;  // List of pre-made platform prefabs with obstacles
    
    [SerializeField] public ObstacleType[] obstacleTypes = new ObstacleType[2];
    
    [Header("Spawn Settings")]
    [SerializeField] public float platformSpawnInterval = 2f;
    [SerializeField] public float despawnDistance = 20f;
    [SerializeField] public float minPointsForPassage = 1.5f; // Minimum safe space to pass through
    
    [Header("Platform Settings")]
    [SerializeField] public float platformWidth = 6f;
    [SerializeField] public float platformHeight = 0.5f;
    [SerializeField] public float platformLength = 6f;  // Doubled from 3
    
    public GameObject GetRandomPlatformPrefab()
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            Debug.LogWarning("No platform prefabs assigned!");
            return null;
        }
        return platformPrefabs[Random.Range(0, platformPrefabs.Length)];
    }
    
    public ObstacleType GetObstacleType(int index)
    {
        if (index < 0 || index >= obstacleTypes.Length)
            return obstacleTypes[0];
        return obstacleTypes[index];
    }
}
