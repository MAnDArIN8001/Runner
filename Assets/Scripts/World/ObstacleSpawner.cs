using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates a large world of platforms with obstacles at game start.
/// Dynamically adds new platforms ahead of the player and removes old ones behind.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private ObstacleConfig config;
    [SerializeField] private Material platformMaterial;
    
    [Header("World Generation")]
    [SerializeField] private float initialWorldLength = 500f; // Total platforms to generate at start
    [SerializeField] private float dynamicSpawnDistance = 100f; // Spawn new platforms when player gets close
    [SerializeField] private float despawnDistance = -50f; // Remove platforms behind player
    
    private PlayerController playerController;
    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastPlatformZ = 0f;
    
    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("ObstacleConfig not assigned!");
            return;
        }
        
        playerController = PlayerController.Instance;
        
        // Generate initial world with many platforms
        GenerateInitialWorld();
    }
    
    private void GenerateInitialWorld()
    {
        if (playerController == null) return;
        
        // Generate platforms from current position to initialWorldLength
        float currentZ = playerController.transform.position.z;
        float targetZ = currentZ + initialWorldLength;
        
        while (currentZ < targetZ)
        {
            CreatePlatformWithObstacles(currentZ);
            currentZ += config.platformLength;
            lastPlatformZ = currentZ;
        }
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGameRunning() || playerController == null) return;
        
        float playerZ = playerController.transform.position.z;
        
        // Spawn new platforms ahead when player gets close to the edge
        if (playerZ + dynamicSpawnDistance > lastPlatformZ)
        {
            float spawnZ = lastPlatformZ + config.platformLength;
            CreatePlatformWithObstacles(spawnZ);
            lastPlatformZ = spawnZ;
        }
        
        // Remove platforms far behind the player
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (activePlatforms[i] != null && activePlatforms[i].transform.position.z < playerZ + despawnDistance)
            {
                Destroy(activePlatforms[i]);
                activePlatforms.RemoveAt(i);
            }
        }
    }
    
    private void CreatePlatformWithObstacles(float platformZ)
    {
        // Get random platform prefab from config
        GameObject platformPrefab = config.GetRandomPlatformPrefab();
        
        if (platformPrefab == null)
        {
            Debug.LogError("No platform prefabs available in config!");
            return;
        }
        
        // Instantiate platform prefab at the specified Z position
        GameObject platformGO = Instantiate(platformPrefab);
        platformGO.name = "Platform";
        platformGO.transform.position = new Vector3(0, 0, platformZ);
        
        // Ensure platform has collider for physics
        if (platformGO.GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = platformGO.AddComponent<BoxCollider>();
            collider.isTrigger = false;
        }
        
        // Ensure platform has rigidbody (kinematic - not affected by physics but still collides)
        if (platformGO.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = platformGO.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Track this platform
        activePlatforms.Add(platformGO);
        GameEvents.InvokeObstacleSpawned(null);
    }
    
    private void SpawnObstaclesOnPlatform(GameObject platform)
    {
        if (config.obstacleTypes.Length == 0)
        {
            Debug.LogWarning("No obstacle types configured!");
            return;
        }
        
        // Decide how many obstacles to spawn (1-3)
        int obstacleCount = Random.Range(1, 4);
        
        // Platform width is 6, divide into lanes: left (-2), center (0), right (2)
        float[] availableLanes = new float[] { -2f, 0f, 2f };
        
        // Randomly select which lanes to block
        for (int i = 0; i < obstacleCount && i < availableLanes.Length; i++)
        {
            if (Random.value > 0.3f) // 70% chance to spawn obstacle in this lane
            {
                SpawnObstacleAtLane(platform, availableLanes[i]);
            }
        }
    }
    
    private void SpawnObstacleAtLane(GameObject platform, float laneX)
    {
        // Select random obstacle type
        int typeIndex = Random.Range(0, config.obstacleTypes.Length);
        ObstacleConfig.ObstacleType obstacleType = config.GetObstacleType(typeIndex);
        
        // Use prefab if available, otherwise skip
        if (obstacleType.prefab == null)
        {
            Debug.LogWarning($"Obstacle prefab for type '{obstacleType.typeName}' is not assigned!");
            return;
        }
        
        // Calculate local position relative to platform
        Vector3 localPos = new Vector3(laneX, config.platformHeight, 0);
        
        // Instantiate as child of platform so it moves together with the platform
        GameObject obstacleGO = Instantiate(obstacleType.prefab, platform.transform);
        obstacleGO.name = obstacleType.typeName;
        obstacleGO.transform.localPosition = localPos;
        
        // Setup obstacle
        Obstacle obstacle = obstacleGO.GetComponent<Obstacle>();
        if (obstacle == null)
        {
            obstacle = obstacleGO.AddComponent<Obstacle>();
        }
        obstacle.Initialize(obstacleType.damage);
        
        GameEvents.InvokeObstacleSpawned(obstacle);
    }
}
