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
    
    void Start()
    { }

    
    void Update()
    {
        if (ViveInput.GetPressDown(role, ControllerButton.Trigger))
        {
            startTracking();
        }

        if (ViveInput.GetPressUp(role, ControllerButton.Trigger))
        {
            endTracking();
        }
    }
    void startTracking()
    {
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
        Quaternion currentRotation = VivePose.GetPose(role).rot;
        float changeInZ = zeroPosition.z - currentRotation.eulerAngles.z;
        
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
