using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reload level
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
