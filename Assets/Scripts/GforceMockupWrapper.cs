using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class GforceMockupWrapper : MonoBehaviour
{
    private int xForce = 10;
    private int yForce = 9;
    private int zForce = -5;
    private float writesPerSecond = 1f;
    private float timeSinceLastUpdate = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        //sendForces();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.LOW);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.LOW);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.LOW);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.LOW);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.LOW);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.HIGH);
            SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.LOW);
        }
    }

    private void sendForces()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= writesPerSecond)
        {
            string send = xForce.ToString() + ":" + yForce.ToString() + ":" + zForce.ToString() + ";";
            timeSinceLastUpdate = 0f;
            SerialConnection.Send(send);
        }
    }
}
