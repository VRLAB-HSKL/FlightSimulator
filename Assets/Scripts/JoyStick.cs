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

    public float tolerance = 5f;
    public ControllerButton trackingActivationButton = ControllerButton.Trigger;
    
    
    
    void Start()
    { }

    private void Update()
    {
        if (ViveInput.GetPressDown(role, trackingActivationButton))
        {
            startTracking();
        }

        if (ViveInput.GetPressUp(role, trackingActivationButton))
        {
            endTracking();
        }
        
    }

    void startTracking()
    {
        tracking = true;
        Quaternion q = VivePose.GetPose(role).rot;
        zeroPosition = q.eulerAngles;
    }

    void endTracking()
    {
        tracking = false;
    }

    public Vector3 getRelativePosition()
    {
        Quaternion q = VivePose.GetPose(role).rot;
        Vector3 delta = zeroPosition - q.eulerAngles;
        delta = ensureDeltaIsInRange(delta);
        return delta;
    }

    /*
     * Check if delta x or y exceed 90/-90 degrees and set them accordingly
     */
    private Vector3 ensureDeltaIsInRange(Vector3 delta)
    {
        if (delta.x >= 0)
        {
            if (delta.x > 90)
            {
                delta.x = 90;
            }
        }
        else
        {
            if (delta.x < -90)
            {
                delta.x = -90;
            }
        }

        if (delta.y >= 0)
        {
            if (delta.y > 90)
            {
                delta.y = 90;
            }
        }
        else
        {
            if (delta.y < -90)
            {
                delta.y = -90;
            }
        }

        //Tolerance for unwanted stick movement
        if (delta.x > -tolerance && delta.x < tolerance)
        {
            delta.x = 0;
        }

        if (delta.y > -tolerance && delta.y < tolerance)
        {
            delta.y = 0;
        }

        return delta;
    }
}
