using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    [Header("item to enable/disable")]
    [SerializeField] private GameObject item;   
    public bool objectEnable;

    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collider)
    {
         if (collider.gameObject.tag == "Player")
        {
           if(objectEnable) 
           {
            item.SetActive(true);
           }
           else
           {
            item.SetActive(false);
           }
        }
    }
}
