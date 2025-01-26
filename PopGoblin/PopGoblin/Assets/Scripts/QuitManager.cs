using UnityEngine;

public class QuitManager : MonoBehaviour
{
    void Update()
   {
      if(Input.GetKeyDown("escape"))
      {
            Quitting();
      }
   }
    public void Quitting()
    {
        Application.Quit();
    }
}
