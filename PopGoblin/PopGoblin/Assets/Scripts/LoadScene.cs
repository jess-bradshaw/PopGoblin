using UnityEngine;
using UnityEngine.SceneManagement; 

public class LoadScene : MonoBehaviour
{
    public Animator transition;
    public Animator bubble;
    
    public void LoadLevel1()
    {
       // StartCoroutine ("Transition");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelA"); 
        
    }

    public void LoadLevel2()
    {
       // StartCoroutine ("Transition");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelB"); 
        
    }
    public void LoadMenu()
    {
        //StartCoroutine ("Transition");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); 
    }

    public void LoadComic()
    {
        //StartCoroutine ("Transition");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Comic");
    }

    // IEnumerator Transition()
    // {
    //    transition.SetTrigger("Start");
    //    bubble.Play("Plane_Transition");
    //     yield return new WaitForSeconds(1f);
    // }
}
