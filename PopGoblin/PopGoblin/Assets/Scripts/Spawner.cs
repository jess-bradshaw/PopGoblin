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
    [Tooltip("Prefab that appears for the pop animation")]
    [SerializeField] private GameObject effectObjectPrefab;

    [Tooltip("How high to move the effect object before spawning the item.")]
    [SerializeField] private float riseAmount = 1f;

    [Tooltip("Time it takes to move up.")]
    [SerializeField] private float riseTime = 1f;

    [Tooltip("Audio source used to play the 'pop' sound.")]
    [SerializeField] private AudioSource popSound;

    [Header("Texture Swap")]
    [Tooltip("Texture used before popping.")]
    [SerializeField] private Material originalTexture;

    [Tooltip("Texture shown at the moment of pop.")]
    [SerializeField] private Material popTexture;

    [Header("Pop Visibility Timing")]
    [Tooltip("How long to keep the pop texture visible before spawn the item and destroy the effect object.")]
    [SerializeField] private float popVisibleTime = 0.5f;

    private void Start()
    {
        //Optionally spawn an item at scene start
        SpawnItem();
    }

    
    private void SpawnItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning($"{name} has no itemPrefab assigned!");
            return;
        }

        GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        DestroyAfterTime destroyScript = newItem.GetComponent<DestroyAfterTime>();

        //Let the new item know which spawner created it
        if (destroyScript != null)
        {
            destroyScript.SetSpawner(this);
        }
        else
        {
            Debug.LogWarning($"Item prefab {itemPrefab.name} is missing DestroyAfterTime component.");
        }
    }

   
    public void StartRespawnSequence()
    {
        Debug.Log($"StartRespawnSequence called on spawner {name}.");
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        //Wait before starting the pop animation
        yield return new WaitForSeconds(respawnDelay);

        //Instantiate the effect prefab, preserving the prefab's rotation
        GameObject effectObjectInstance = null;
        if (effectObjectPrefab != null)
        {
            effectObjectInstance = Instantiate(
                effectObjectPrefab,
                transform.position,
                effectObjectPrefab.transform.rotation
            );

            //Optionally set the original texture
            Renderer effectRenderer = effectObjectInstance.GetComponent<Renderer>();
            if (effectRenderer != null && originalTexture != null)
            {
                effectRenderer.material = originalTexture;
            }

            //Animate it rising up
            yield return StartCoroutine(RiseEffectObject(effectObjectInstance));

            //Play pop sound and switch texture
            if (popSound != null)
            {
                popSound.Play();
            }
            if (effectRenderer != null && popTexture != null)
            {
                effectRenderer.material = popTexture;
            }

            //Wait so the pop texture is visible for a bit
            yield return new WaitForSeconds(popVisibleTime);

            //Spawn the new item at the effect object's position
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

            //Destroy the effect object
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
