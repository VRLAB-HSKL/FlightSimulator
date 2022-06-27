using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GforceMockupWrapper : MonoBehaviour
{
    private SerialConnection sc;
    private int xForce = 10;
    private int yForce = 9;
    private int zForce = -5;
    private int writesPerSecond = 1;
    private float timeSinceLastUpdate = 0.0f;
    void Start()
    {
        sc = SerialConnection.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= writesPerSecond)
        {
            string send = "*" + xForce.ToString() + ":" + yForce.ToString() + ":" + zForce.ToString() + ";";
            sc.Send(send);
        }
    }
}
