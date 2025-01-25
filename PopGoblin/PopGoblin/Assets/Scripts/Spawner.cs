using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Item Prefab & Timing")]
    [Tooltip("The item prefab to spawn. Must have DestroyAfterTime script attached.")]
    [SerializeField] private GameObject itemPrefab;

    [Tooltip("How long to wait after the old item is destroyed before popping a new one.")]
    [SerializeField] private float respawnDelay = 2f;

    [Header("Pop Effect (Prefab)")]
    [Tooltip("Prefab that appears for the pop animation (plane, quad, etc.).")]
    [SerializeField] private GameObject effectObjectPrefab;

    [Tooltip("How high to move the effect object before spawning the item.")]
    [SerializeField] private float riseAmount = 1f;

    [Tooltip("Time it takes to move up.")]
    [SerializeField] private float riseTime = 1f;

    [Tooltip("Audio source used to play the 'pop' sound.")]
    [SerializeField] private AudioSource popSound;

    [Header("Texture Swap (Optional)")]
    [Tooltip("Texture used before popping.")]
    [SerializeField] private Texture originalTexture;

    [Tooltip("Texture shown at the moment of pop.")]
    [SerializeField] private Texture popTexture;

    [Header("Pop Visibility Timing")]
    [Tooltip("How long to keep the pop texture visible before we spawn the item and destroy the effect object.")]
    [SerializeField] private float popVisibleTime = 0.5f;

    private void Start()
    {
        // Optionally spawn an item at scene start
        SpawnItem();
    }

    /// <summary>
    /// Spawns a new item and sets its spawner reference.
    /// </summary>
    private void SpawnItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning($"{name} has no itemPrefab assigned!");
            return;
        }

        GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        DestroyAfterTime destroyScript = newItem.GetComponent<DestroyAfterTime>();

        // Let the new item know which spawner created it
        if (destroyScript != null)
        {
            destroyScript.SetSpawner(this);
        }
        else
        {
            Debug.LogWarning($"Item prefab {itemPrefab.name} is missing DestroyAfterTime component.");
        }
    }

    /// <summary>
    /// Called by the item (DestroyAfterTime) when it's destroyed.
    /// Starts the "pop" sequence, then spawns a new item afterward.
    /// </summary>
    public void StartRespawnSequence()
    {
        Debug.Log($"StartRespawnSequence called on spawner {name}.");
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        // 1) Wait before starting the pop animation
        yield return new WaitForSeconds(respawnDelay);

        // 2) Instantiate the effect prefab, preserving the prefab's rotation
        GameObject effectObjectInstance = null;
        if (effectObjectPrefab != null)
        {
            effectObjectInstance = Instantiate(
                effectObjectPrefab,
                transform.position,
                effectObjectPrefab.transform.rotation
            );

            // Optionally set the original texture
            Renderer effectRenderer = effectObjectInstance.GetComponent<Renderer>();
            if (effectRenderer != null && originalTexture != null)
            {
                effectRenderer.material.mainTexture = originalTexture;
            }

            // Animate it rising up
            yield return StartCoroutine(RiseEffectObject(effectObjectInstance));

            // 3) Play pop sound and switch texture
            if (popSound != null)
            {
                popSound.Play();
            }
            if (effectRenderer != null && popTexture != null)
            {
                effectRenderer.material.mainTexture = popTexture;
            }

            // 4) Wait so the pop texture is visible for a bit
            yield return new WaitForSeconds(popVisibleTime);

            // 5) Spawn the new item at the effect object's position
            Vector3 spawnPos = effectObjectInstance.transform.position;
            GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            if (newItem != null)
            {
                DestroyAfterTime destroyScript = newItem.GetComponent<DestroyAfterTime>();
                if (destroyScript != null)
                {
                    destroyScript.SetSpawner(this);
                }
            }

            // 6) Destroy the effect object
            Destroy(effectObjectInstance);
        }
        else
        {
            Debug.LogWarning($"{name} has no effectObjectPrefab assigned!");
        }
    }

    private IEnumerator RiseEffectObject(GameObject effectObject)
    {
        Vector3 startPos = effectObject.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseAmount;
        float elapsed = 0f;

        while (elapsed < riseTime)
        {
            float t = elapsed / riseTime;
            effectObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectObject.transform.position = endPos;
    }
}
