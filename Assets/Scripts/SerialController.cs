using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialController : MonoBehaviour
{
    private SerialReader reader;
    
    // Start is called before the first frame update
    void Start()
    {
        reader = new SerialReader();
        reader.active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
