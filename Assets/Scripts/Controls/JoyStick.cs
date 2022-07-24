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
    public Transform flightStickModel;

    private Quaternion zeroPositionQ;
    private Vector3 zeroPosition;
    private Vector2 delta;
    private Vector3 zeroAxisRotation;
    private Vector3 zeroAngle;
    private Quaternion flightStickModelZeroRotation;

    #region Debug

    private float timesincelastlog = 0f;
    private float timebetweenlogs = 1f;

    #endregion

    private void Start()
    {
        flightStickModelZeroRotation = flightStickModel.localRotation;
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
            flightStickModel.localRotation = flightStickModelZeroRotation;
        }

        if (tracking)
        {
            calculateRelativePosition();
            updateStickModel();
        }

    }

    private void updateStickModel()
    {
        /*
        Quaternion current = VivePose.GetPose(role).rot;  //controllerTransform.rotation;
        Quaternion differenceInRotation = Quaternion.Inverse(current) * flightStickModelZeroRotation;
        flightStickModel.localRotation = differenceInRotation;
        */
    }


    void startTracking()
    {
        tracking = true;
        zeroPositionQ = VivePose.GetPose(role).rot;
    }

    void endTracking()
    {
        tracking = false;
        delta = Vector2.zero;
        
    }
    
    /// <summary>
    /// 
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
    

    private void logToConsole(Vector3 currentEuler)
    {
        timesincelastlog += Time.deltaTime;
        if (timesincelastlog > timebetweenlogs)
        {
            timesincelastlog = 0f;
            Debug.Log("Euler: " + currentEuler);
        }
    }

    public Vector2 getRelativePosition()
    { 
        return new Vector2(delta.x/range, delta.y/range);
    }
    
}
