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
        if (Input.GetKeyDown(KeyCode.E))
        {
            SerialConnection.setOutputOnPort(SerialConnection.LED_PORT, SerialConnection.HIGH);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SerialConnection.setOutputOnPort(SerialConnection.LED_PORT, SerialConnection.LOW);
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
