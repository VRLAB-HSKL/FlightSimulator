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

    

    public HandRole role;
    public Transform controllerTransform;
    public ControllerButton trackingActivationButton = ControllerButton.Trigger;
    public float range;
    public float tolerance = 0.5f;
   
    private Vector3 zeroPosition;
    private Vector2 delta;
    private Vector3 zeroAxisRotation;
    private Vector3 zeroAngle;

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

        if (tracking)
        {
            calculateRelativePosition();
        }

    }


    void startTracking()
    {
        tracking = true;
        Quaternion q = controllerTransform.localRotation;
        zeroPosition = q.eulerAngles;
    }

    void endTracking()
    {
        tracking = false;
        delta = Vector2.zero;
        
    }
    
    private void calculateRelativePosition()
    {
        Quaternion current = controllerTransform.localRotation;
        Vector3 currentEuler = current.eulerAngles;
        Vector3 deltaV3 = zeroPosition - currentEuler;
        float deltaX = deltaV3.x;
        float deltaY = deltaV3.z;
        if (deltaX > 180) deltaX -= 360;
        if (deltaX < -180) deltaX += 360;

        if (deltaY > 180) deltaY -= 360;
        if (deltaY < -180) deltaY += 360;

        Mathf.Clamp(deltaX, -90, 90);
        Mathf.Clamp(deltaY, -90, 90);
        if (delta.x > -tolerance && delta.x < tolerance)
        {
            delta.x = 0;
        }
        if (delta.y > -tolerance && delta.y < tolerance)
        {
            delta.y = 0;
        }
        delta.x = deltaX;
        delta.y = deltaY;
    }

    public Vector2 getRelativePosition()
    {
        return new Vector2(delta.x/range, delta.y/range);
    }
    
}
