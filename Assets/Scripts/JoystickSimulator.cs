using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;
using Valve.VR.InteractionSystem;

public class JoystickSimulator : MonoBehaviour
{
    private Vector2 joyStickZeroPosition;
    private float maxJoyStickPosition = 90;
    private float maxThrottlePosition = 90;
    private Vector3 throttleZeroPosition;

    private bool joyStickTracking = false;

    private bool throttleTracking = false;
    // Start is called before the first frame update
    void Start()
    {
        InputRegistry.OnLeftTriggerPressed += startThrottleTracking;
        InputRegistry.OnRightTriggerPressed += startJoyStickTracking;
        InputRegistry.OnLeftTriggerReleased += endThrottleTracking;
        InputRegistry.OnRightTriggerReleased += endJoyStickTracking;
    }

   
    public Vector3 getJoyStickPosition()
    {
        return Vector3.zero;
    }

    /**
     * Returns vektor2 [range(1,-1),range(1,-1)]
     * 0,0
     */
    public Vector2 getJoyStickDelta()
    {
        float xAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickX);
        float yAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickY);
        Vector2 delta = new Vector2(joyStickZeroPosition.x - xAxis, joyStickZeroPosition.y - yAxis);
        return delta;
    }

    public float getRelativeThrottlePosition()
    {
        Vector3 currentThrottlePosition = HTCViveControllerProfile.ViveController.leftHand.devicePosition.ReadValue();
        return Mathf.Abs(throttleZeroPosition.x - currentThrottlePosition.x);
    }

    void startJoyStickTracking()
    {
        joyStickTracking = true;
        float xAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickX);
        float yAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickY);
        joyStickZeroPosition = new Vector2(xAxis, yAxis);
    }

    void startThrottleTracking()
    {
        throttleTracking = true;
        throttleZeroPosition = HTCViveControllerProfile.ViveController.leftHand.devicePosition.ReadValue();
    }

    void endJoyStickTracking()
    {
        joyStickTracking = false;

    }

    void endThrottleTracking()
    {
        throttleTracking = false;
    }

    private void OnDestroy()
    {
        InputRegistry.OnLeftTriggerPressed -= startThrottleTracking;
        InputRegistry.OnRightTriggerPressed -= startJoyStickTracking;
        InputRegistry.OnLeftTriggerReleased -= endThrottleTracking;
        InputRegistry.OnRightTriggerReleased -= endJoyStickTracking;
    }
}
