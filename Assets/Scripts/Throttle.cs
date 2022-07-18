using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class Throttle : MonoBehaviour
{
    public bool tracking = false;

    public HandRole role;
    public ControllerButton trackingActivationButton = ControllerButton.Trigger;
    public float rangeInMeters;
    public float currentPercentage = 0f;
    public bool invert = true;
    private Vector3 zeroPosition;
    private float throttleValue = 0f;
    void Update()
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
            adjustThrottleValue();
        }
    }
    void startTracking()
    {
        tracking = true;
        zeroPosition = VivePose.GetPose(role).pos;
    }

    void endTracking()
    {
        tracking = false;
    }

    void adjustThrottleValue()
    {
        
        Vector3 currentPosition = VivePose.GetPose(role).pos;
        float changeInZ = (zeroPosition.z - currentPosition.z);
        float newPercentage = Mathf.Clamp(changeInZ / rangeInMeters, -1 ,1);
        if (0.01 >= newPercentage && newPercentage >= -0.01)
        {
            //change is too low to adjust throttle
            return;
        }
        throttleValue = Mathf.Clamp01(throttleValue + newPercentage);
        currentPercentage = throttleValue;

    }
    public float getThrottleValue()
    {
        if (invert)
        {
            return -throttleValue;
        }
        return throttleValue;
    }
}
