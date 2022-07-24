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

    private Quaternion zeroPositionQ;
    private Vector3 zeroPosition;
    private Vector2 delta; // difference in rotation 
    private Vector3 zeroAxisRotation;
    private Vector3 zeroAngle;
    private Quaternion flightStickModelZeroRotation;
    
    #region Unity lifecycle

    private void Start()
    {
        range = Mathf.Abs(range) * -1;
    }

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
    #endregion

    #region public
    /// <summary>
    /// 
    /// </summary>
    /// <returns>The relative difference in rotation in proportion to the set range</returns>
    public Vector2 getRelativePosition()
    { 
        return new Vector2(delta.x/range, delta.y/range);
    }

    #endregion

    #region private

    /// <summary>
    /// Sets tracking flag to true and logs the current rotation as zeroPosition
    /// </summary>
    void startTracking()
    {
        tracking = true;
        zeroPositionQ = VivePose.GetPose(role).rot;
    }

    /// <summary>
    /// Ends tracking and sets delta to (0,0)
    /// </summary>
    void endTracking()
    {
        tracking = false;
        delta = Vector2.zero;
    }

    /// <summary>
    /// Calculates the difference in rotation between the current controller rotation and the controller zero position.
    /// Sets Delta accordingly. Ignores rotation in the y axis.
    /// </summary>
    private void calculateRelativePosition()
    {
        // flight stick x has a range from -180 to 0
        Quaternion current = VivePose.GetPose(role).rot;  //controllerTransform.rotation;
        Quaternion differenceInRotation = Quaternion.Inverse(current)*zeroPositionQ;

        Vector3 euler = differenceInRotation.eulerAngles;
        
        if (euler.x > 180) euler.x -= 360;
        if (euler.y > 180) euler.y -= 360;
        
        delta.x = euler.x;
        delta.y = euler.y;
    }
    #endregion

}
