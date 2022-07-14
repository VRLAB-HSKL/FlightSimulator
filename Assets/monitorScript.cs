
using UnityEngine;

public class monitorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    
    FindObjectOfType<AudioManager>().Play("CockpitIndoorSound");
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
