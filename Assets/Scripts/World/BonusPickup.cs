using UnityEngine;

/// <summary>
/// Runtime pickup on a trigger collider. <see cref="PlayerController"/> applies the definition on trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BonusPickup : MonoBehaviour
{
    [SerializeField] private BonusPickupDefinition definition;

    public BonusPickupDefinition Definition => definition;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
        
    }

    private void Awake()
    {
        var c = GetComponent<Collider>();
        if (c != null)
            c.isTrigger = true;
    }
}
