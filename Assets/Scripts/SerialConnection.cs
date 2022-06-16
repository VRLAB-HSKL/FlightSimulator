using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialConnection : MonoBehaviour
{
    private SerialPort sp;

    private const string portName = "COM3";

    private const int baudRate = 9600;
    // Start is called before the first frame update
    void Start()
    {
        sp = new SerialPort(portName, baudRate);
    }
    
}
