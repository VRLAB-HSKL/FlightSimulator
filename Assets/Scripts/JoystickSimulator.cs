using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class JoystickSimulator : MonoBehaviour
{
    private Vector2 joyStickZeroPosition;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getJoyStickPosition()
    {
        return Vector3.zero;
    }

    public Vector2 getJoyStickDelta()
    {
        float xAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickX);
        float yAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickY);
        Vector2 delta = new Vector2(Mathf.Abs(joyStickZeroPosition.x - xAxis), Mathf.Abs(joyStickZeroPosition.y - yAxis));
        return delta;
    }

    public float getRelativeThrottlePosition()
    {
        Vector3 currentThrottlePosition = HTCViveControllerProfile.ViveController.leftHand.devicePosition.ReadValue();
        return Mathf.Abs(throttleZeroPosition.x - currentThrottlePosition.x);
    }

    void startJoyStickTracking()
    {
        float xAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickX);
        float yAxis = ViveInput.GetAxis(HandRole.RightHand, ControllerAxis.JoystickY);
        joyStickZeroPosition = new Vector2(xAxis, yAxis);
    }

    void startThrottleTracking()
    {
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
