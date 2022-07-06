using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;


public class JoyStick : MonoBehaviour
{
    public bool tracking = false;

    private Vector3 zeroPosition;

    public HandRole role;
    
    
    void Start()
    {
        InputRegistry.OnRightTriggerPressed += startTracking;
        InputRegistry.OnRightTriggerReleased += endTracking;
    }
    
    void startTracking()
    {
        Quaternion q = VivePose.GetPose(HandRole.RightHand).rot;
        zeroPosition = q.eulerAngles;
    }

    void endTracking()
    {
        tracking = false;
    }

    Vector3 getRelativePosition()
    {
        Quaternion q = VivePose.GetPose(HandRole.RightHand).rot;
        return zeroPosition - q.eulerAngles;
    }

    private void OnDestroy()
    {
        InputRegistry.OnRightTriggerPressed -= startTracking;
        InputRegistry.OnRightTriggerReleased -= endTracking;
    }
}
