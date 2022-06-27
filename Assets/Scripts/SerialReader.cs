using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialReader : MonoBehaviour
{
    public bool active = true;

    private string currentLine = "";

    private string endLiteral = ";";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            currentLine += SerialConnection.instance.readExisting();
            if (currentLine.Contains(endLiteral))
            {
                Debug.Log(currentLine);
                currentLine = "";
            }
        }
    }
}
