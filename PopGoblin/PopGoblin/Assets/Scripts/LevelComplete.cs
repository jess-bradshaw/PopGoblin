using UnityEngine;
using System.Collections;

public class LevelComplete : MonoBehaviour
{
    public float delayInSeconds = 5f;

    public GameObject targetUI;

    private void OnEnable()
    {
        if (targetUI != null)
        {
            StartCoroutine(EnableAfterDelay());
        }
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);

        targetUI.SetActive(true);

    
    }
}
