using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class JoystickSimulator : MonoBehaviour
{
    private Vector3 flightStickZeroPosition;

    private Vector3 throttleZeroPosition;
    // Start is called before the first frame update
    void Start()
    {
        InputRegistry.OnLeftTriggerPressed += startThrottleTracking;
        InputRegistry.OnRightTriggerPressed += startJoyStickTracking;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startJoyStickTracking()
    {
        float xAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickX);
        float yAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickY);
        flightStickZeroPosition = new Vector3(xAxis, yAxis);
    }

    void startThrottleTracking()
    {
        
    }

    private void OnDestroy()
    {
        InputRegistry.OnLeftTriggerPressed -= startThrottleTracking;
        InputRegistry.OnRightTriggerPressed -= startJoyStickTracking;
    }
}
