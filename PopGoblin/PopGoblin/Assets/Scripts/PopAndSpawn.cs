using UnityEngine;
using System.Collections;

public class ThoughtBubblePop : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Tag of the thrown item that can pop this bubble.")]
    [SerializeField] private string popItemTag = "PopItem";

    [Header("Bubble Sprite Swap")]
    [Tooltip("The SpriteRenderer for this bubble.")]
    [SerializeField] private SpriteRenderer bubbleSpriteRenderer;

    [Tooltip("Sprite to display once this bubble is popped.")]
    [SerializeField] private Sprite bubblePoppedSprite;

    [Header("New Bubble to Spawn")]
    [Tooltip("Prefab for the new bubble that grows after popping.")]
    [SerializeField] private GameObject newBubblePrefab;

    [Tooltip("Initial scale for the new bubble.")]
    [SerializeField] private Vector3 newBubbleStartScale = Vector3.zero;

    [Tooltip("Final scale for the new bubble after it finishes growing.")]
    [SerializeField] private Vector3 newBubbleFinalScale = Vector3.one;

    [Tooltip("How many seconds it takes for the new bubble to grow from start to final scale.")]
    [SerializeField] private float newBubbleGrowthDuration = 2f;

    [Header("NPC Sprite Swap")]
    [Tooltip("The NPC's SpriteRenderer whose image you want to change.")]
    [SerializeField] private SpriteRenderer npcSpriteRenderer;

    [Tooltip("The new sprite for the NPC after the bubble has fully grown.")]
    [SerializeField] private Sprite npcNewSprite;

    [SerializeField] private float poppedVisibilityDuration = 1f;

    // Prevent double-popping
    private bool hasPopped = false;

    /// <summary>
    /// If this bubble has a standard (non-trigger) 3D collider,
    /// we'll detect collisions here. If your bubble is a trigger, 
    /// change to OnTriggerEnter(Collider other).
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Only pop once
        if (hasPopped) return;

        // Check if the collided item has the correct tag
        if (collision.gameObject.CompareTag(popItemTag))
        {
            hasPopped = true;
            PopBubble();
        }
    }

    /// <summary>
    /// Called when we detect the correct pop item collision.
    /// 1) Swap this bubble's sprite to the popped sprite.
    /// 2) Spawn a new bubble and grow it.
    /// </summary>
    private void PopBubble()
    {
        // 1) Swap bubble sprite
        if (bubbleSpriteRenderer != null && bubblePoppedSprite != null)
        {
            bubbleSpriteRenderer.sprite = bubblePoppedSprite;
        }


        // 2) Spawn the new bubble (if assigned)
        if (newBubblePrefab != null)
        {
            // Create it at the same position as this bubble
            GameObject newBubble = Instantiate(newBubblePrefab, transform.position, Quaternion.identity);

            // Set initial scale to zero (or whatever you specified)
            newBubble.transform.localScale = newBubbleStartScale;

            // Animate it growing
            StartCoroutine(GrowNewBubble(newBubble.transform));
        }
    }

    /// <summary>
    /// Grows the newly spawned bubble from 'newBubbleStartScale' to 'newBubbleFinalScale'
    /// over 'newBubbleGrowthDuration'. Once it finishes, we swap the NPC's sprite.
    /// </summary>
    private IEnumerator GrowNewBubble(Transform bubbleTransform)
    {
        yield return new WaitForSeconds(poppedVisibilityDuration);

        // 3. Make the bubble invisible
        if (bubbleSpriteRenderer != null)
        {
            bubbleSpriteRenderer.enabled = false;
            Debug.Log("ThoughtBubblePop: Popped bubble is now invisible.");
        }

        float elapsed = 0f;
        while (elapsed < newBubbleGrowthDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / newBubbleGrowthDuration;
            bubbleTransform.localScale = Vector3.Lerp(newBubbleStartScale, newBubbleFinalScale, t);
            yield return null;
        }

        // Ensure it's at final scale
        bubbleTransform.localScale = newBubbleFinalScale;

        // Once finished growing, change the NPC's sprite
        if (npcSpriteRenderer != null && npcNewSprite != null)
        {
            npcSpriteRenderer.sprite = npcNewSprite;
        }
    }
}
