using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [Header("Lifetime Settings")]
    [Tooltip("items life before respawn.")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Start Respawn System - remove Update function after pickup script")]
    [Tooltip("remove update function once pick is working")]
    [SerializeField]
    private bool canDestroy = false;

    
    //stop multiple happening
    private bool destructionScheduled = false;

    // Reference to the spawner that created this item.
    private Spawner spawner;

    //this is for testing purposes only, remove one pick is done
    void Update()
    {
        // If canDestroy is true AND we haven't started the timer yet...
        if (canDestroy && !destructionScheduled)
        {
            destructionScheduled = true;
            Invoke(nameof(DestroySelf), lifeTime);
        }
    }

    public void SetSpawner(Spawner spawner)
    {
        this.spawner = spawner;
    }

    public void BeginDestructionTimer()
    {
        if (!destructionScheduled)
        {
            destructionScheduled = true;
            canDestroy = true;

            // Schedule DestroySelf() to be called after lifeTime seconds.
            Invoke(nameof(DestroySelf), lifeTime);
        }
    }

    private void DestroySelf()
    {
        // Tell the spawner to begin its "pop + respawn" sequence.
        if (spawner != null)
        {
            spawner.StartRespawnSequence();
            Debug.Log("Im respawning");
        }

        // Destroy this item from the scene.
        Destroy(gameObject);
        Debug.Log($"DestroySelf called on {gameObject.name}.");
    }
}
