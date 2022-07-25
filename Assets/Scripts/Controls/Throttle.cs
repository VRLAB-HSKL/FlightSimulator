using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class Throttle : MonoBehaviour
{
    #region public attributes

    public HandRole role;
    public ControllerButton trackingActivationButton = ControllerButton.Trigger;
    public float rangeInMeters;
    public bool invert = false;

    #endregion

    #region private/readonly

    private Vector3 zeroPosition;
    private float throttleValue = 0f;
    public bool tracking { get; private set; }
    
    #endregion
    
    #region Unity Lifecycle

    private void Start()
    {
        if (invert) throttleValue = 1f;
    }

    void Update()
    {
        //register input
        if (ViveInput.GetPressDown(role, trackingActivationButton))
        {
            startTracking();
        }

        if (ViveInput.GetPressUp(role, trackingActivationButton))
        {
            endTracking();
        }
        
        //process input
        if (tracking)
        {
            adjustThrottleValue();
        }
    }

    #endregion

    #region public

    /// <summary>
    /// Returns throttle value based on the invert flag.
    /// </summary>
    /// <returns>The current throttle value</returns>

    public float getThrottleValue()
    {
        if (invert)
        {
            return 1 - throttleValue;
        }
        return throttleValue;
    }

    #endregion

    #region Private
    /// <summary>
    /// Registers the controller position at the time of the button press as new zero position.
    /// </summary>
    void startTracking()
    {
        tracking = true;
        zeroPosition = VivePose.GetPose(role).pos;
    }

    void endTracking()
    {
        tracking = false;
    }

    /// <summary>
    /// Adjusts the throttle value based on the delta between the current controller position and the zeroPosition in the z axis.
    /// </summary>
    void adjustThrottleValue()
    {
        //get current position
        Vector3 currentPosition = VivePose.GetPose(role).pos;
        //get delta
        float changeInZ = (zeroPosition.z - currentPosition.z);
        //get percentage moved in regards to the set range and clamp it between -1 and 1
        float newPercentage = Mathf.Clamp(changeInZ / rangeInMeters, -1 ,1);
        //ignore changes if to low, could only be hand shaking and no intended movement
        if (0.01 >= newPercentage && newPercentage >= -0.01)
        {
            //change is too low to adjust throttle
            return;
        }
        //add the value on top of the current value and clamp it between 0 and 1
        throttleValue = Mathf.Clamp01(throttleValue + newPercentage);
    }
    #endregion
    
    
}
