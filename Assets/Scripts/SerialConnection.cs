using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class SerialConnection : MonoBehaviour
{
    #region constants
    public const int MOTOR1_PORT = 0;
    public const int MOTOR2_PORT = 1;
    public const int MOTOR3_PORT = 2;
    public const int LED_PORT = 13;
    public const int HIGH = 1;
    public const int LOW = 0;
    private const string setOperationChar = ">";
    #endregion

    #region  exposedVariables
    public string portName = "COM3";
    public int baudRate = 9600;
    public bool readFromSerial = true;
    #endregion

    #region privateMembers
    private static SerialPort stream;
    private float lastRead;
    private Thread readThread;
    #endregion

    #region UnityLifecycle
    private void Awake()
    {
        //configure and open serial port
        stream = new SerialPort(portName, baudRate);
        stream.ReadTimeout = 500;
        stream.WriteTimeout = 500;
        stream.Open();
        
        //start the read thread
        readThread = new Thread(read);
        readThread.Start();
    }
    private void Update()
    {
        
    }
    private void OnDestroy()
    {
        stream.Close();
    }
    #endregion
    
    #region communicationFunctions
    public static void Send(string s)
    {
        stream.Write(s);
        Debug.Log("Send value: " + s + "over serialconnection");
    }

    public void read()
    {
        while (true)
        {
            if (readFromSerial)
            {
                try
                {
                    string value = "";
                    value = stream.ReadLine();
                    if (value != "") Debug.Log("Serialport: " + value);
                }
                catch (TimeoutException)
                {
                    //timeout will happen from time to time
                    //since we only use it to log the state of the arduino to console we can ignore it
                }
            }
        }
    }

    public static void setOutputOnPort(int port, int value)
    {
        if (port != LED_PORT && port != MOTOR1_PORT && port != MOTOR2_PORT && port != MOTOR3_PORT)
        {
            throw new ArgumentException("Port '" + port +"' is no valid port");
        }

        if (value != LOW && value != HIGH)
        {
            throw new ArgumentException("Value '" + value  +"' is no valid Value");
        }
        Debug.Log("Writing: " + setOperationChar + port + ":" + value);
        stream.WriteLine(setOperationChar + port + ":" + value);
    }
    #endregion
    
}