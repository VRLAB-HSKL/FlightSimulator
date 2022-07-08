using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class Throttle : MonoBehaviour
{
    public bool tracking = false;

    private Vector3 zeroPosition;
    private float throttleValue; // percentage from -1 to 1

    public HandRole role;
    public ControllerButton trackingActivationButton = ControllerButton.Trigger;

    void Start()
    {
        
    }

    
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

    /*
     * 1 = full speed forward
     * -1 = full speed backwards
     * 0 = no speed
     * @return - Throttlevalue in percent in range(-1,1) 
     */
    public float getThrottleValue()
    {
        Vector3 currentPosition = VivePose.GetPose(role).pos;
        float changeInZ = (zeroPosition.z - currentPosition.z) * 100 * 3;
        
        if (changeInZ < 0)
        {
            if (changeInZ < -90) changeInZ = -90;
        }

        if (changeInZ > 0)
        {
            if (changeInZ > 90) changeInZ = 90;
        }
        throttleValue = changeInZ / 90;
        return throttleValue;
    }
}
