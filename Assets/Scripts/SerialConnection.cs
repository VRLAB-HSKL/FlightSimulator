using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using UnityEngine;

public class SerialConnection : MonoBehaviour
{
    public static SerialConnection instance;
    private static SerialPort sp;


    private const string portName = "COM3";

    private const int baudRate = 9600;

    // Start is called before the first frame update
    private const float readIntervalperSecond = 0.3f;
    private float lastRead;
    

    private void Awake()
    {
        sp = new SerialPort(portName, baudRate);
        instance = this;
        sp.Open();
        sp.ReadTimeout = 200;
    }

    private void Update()
    {
    }


    public void Send(string s)
    {
        sp.Write(s);
    }

    public String readExisting()
    {
        string value = null; 
        value = sp.ReadExisting();
        return value;

    }

    private void OnDestroy()
    {
        sp.Close();
    }
}