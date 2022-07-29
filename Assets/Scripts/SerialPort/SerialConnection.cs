using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;


public class SerialConnection : MonoBehaviour
{
    #region constants

    public const int maxPortNumber = 13;
    public const int minPortNumber = 2;
    public const int MOTOR1_PORT_SET = 12;
    public const int MOTOR1_PORT_RESET = 11;
    public const int MOTOR2_PORT_SET = 10;
    public const int MOTOR2_PORT_RESET = 9;
    public const int MOTOR3_PORT_SET = 8;
    public const int MOTOR3_PORT_RESET = 7;
    public const int LED_PORT = 13;
    private const string setOperationChar = ">";
    private const int delay = 50;

    public enum State
    {
        LOW = 0,
        HIGH = 1
    }
    #endregion

    #region  exposedVariables
    public string portName = "COM3";
    public int baudRate = 9600; // bit per second
    public bool readFromSerial = true; // should incoming data be read or ignored
    #endregion

    #region privateMembers
    public static SerialPort stream; // the actual serialport
    private float lastRead;
    private Thread readThread;
    #endregion

    #region UnityLifecycle
    private void Awake()
    {
        //configure and open serial port
        stream = new SerialPort(portName, baudRate);
        //always set timeouts to prevent unity from crashing
        stream.ReadTimeout = 500;
        stream.WriteTimeout = 500;
        stream.Open();
        
        //start the read thread
        readThread = new Thread(read);
        readThread.Start();
    }
    private void OnDestroy()
    {
        stream.Close();
    }
    #endregion
    
    #region communicationFunctions
    /// <summary>
    /// Reads incoming data from the serialport and logs it to console, if the corresponding flag is set.
    /// </summary>
    private void read()
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
                    //if a future human being wants to do something with incoming data a more thought out error handling is required
                }
            }
        }
    }

    /// <summary>
    /// Sets the output on the corresponding pin to the specified value. The send command string consists of 5 characters.
    /// This means every time this function gets called 40 bit will be send over the serial port. 
    /// </summary>
    /// <param name="pin">Pin number, allowed range for the arduino uno r3 is 2-13, 13 corresponds to the led builtin</param>
    /// <param name="value">High or Low</param>
    /// <exception cref="ArgumentException">Gets thrown if port or value is not in allowed range of values</exception>
    public static void setOutputOnPin(int pin, State value)
    {
        
        if (pin < minPortNumber|| pin > maxPortNumber)
        {
            throw new ArgumentException("Port '" + pin +"' is no valid port");
        }

        int valueNumber;
        switch (value)
        {
            case State.LOW:
                valueNumber = 0;
                break;
            case State.HIGH:
                valueNumber = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
        if (stream.IsOpen)
        {
            stream.WriteLine(setOperationChar + pin + ":" + valueNumber);    
        }
        else
        {
            Debug.Log("Stream is not open");
        }
        
    }

    /// <summary>
    /// Resets all digital pins on the arduino board to 0.
    /// </summary>
    public static void resetAll()
    {
        stream.WriteLine("r");
    }
    #endregion
    
}