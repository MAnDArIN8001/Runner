using UnityEngine;

/// <summary>
/// Base obstacle class - handle collision and damage.
/// Can be instantiated from prefab or created dynamically.
/// </summary>
public class Obstacle : MonoBehaviour
{
    private int damage = 10;

    public GameObject pufVfx;
    
    public int GetDamage() => damage;
    
    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }
    
    public void Initialize(int damageAmount)
    {
        damage = damageAmount;
        
        // Ensure collider is a trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // Ensure rigidbody is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
    
    /// <summary>
    /// Breaks the obstacle - plays break animation/effect and removes from game.
    /// </summary>
    public void Break()
    {
        Debug.Log($"Obstacle '{gameObject.name}' broken!");
        
        Instantiate(pufVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
