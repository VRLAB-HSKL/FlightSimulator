using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


[RequireComponent(typeof(JoyStick))]
[RequireComponent(typeof(Throttle))]
[RequireComponent(typeof(FlightPhysics))]
public class PlaneController : MonoBehaviour
{
    #region Public_Members
    public JoyStick joyStick;
    public Throttle throttle;
    
    public float accelerationPerSecond = 20f;
    public float maxSpeedPerSec = 343f; // 1fache schallgeschwindigkeit

    public GameObject throttleObject;
    public GameObject flightStickObject;
    
    // gameobject for sound
    public GameObject monitor; // Sound should come out of the monitors
    public float currentSpeed = 0f; 
    public float speedGoal = 0f; // goal in range(0, maxspeedpersec)

    public FlightPhysics flightPhysics;
    #endregion

    #region Private_Members
    
    
    private Vector3 currentRotationPerSecond = Vector3.zero;
    private Vector3 maxRotationPerSecond = new Vector3(50, 0, 50); //x: Pitch z: Roll 

    private Rigidbody jetBody;

    private Vector3 flightStickRotation;
    private Vector2 delta;
    #endregion

    #region TestVariables

    
    private Quaternion cameraZeroRotation= Quaternion.Euler(0,0,0 );

    #endregion
    
    


    #region Unity_LifeCycle
    void Start()
    {
        jetBody = GetComponent<Rigidbody>();
        flightPhysics = GetComponent<FlightPhysics>();

    }

    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float pitchPercentage = 0f;
        float rollPercentage = 0f;
        //alignPlaneWithHMD();
        
        if (throttle.tracking)
        {
            speedGoal = maxSpeedPerSec * throttle.getThrottleValue();
        }

        if (speedGoal > currentSpeed)
        {
            currentSpeed += accelerationPerSecond;
            if (currentSpeed > maxSpeedPerSec) currentSpeed = maxSpeedPerSec;
        }else if (speedGoal < currentSpeed)
        {
            currentSpeed -= accelerationPerSecond;
            if (currentSpeed < 0) currentSpeed = 0;
        }
        
        if (joyStick.tracking)
        {
            delta = joyStick.getRelativePosition();
            pitchPercentage = delta.x;
            rollPercentage = delta.y;
            
            
            currentRotationPerSecond = new Vector3(
                maxRotationPerSecond.x * pitchPercentage * Time.deltaTime,
                0,
                maxRotationPerSecond.z * rollPercentage * Time.deltaTime);

        }
        else
        {
            currentRotationPerSecond = Vector3.zero;
            //todo stop rotating smoothly
        }
        
        //updatePlane();

        
        if (joyStick.tracking)
        {
            delta = joyStick.getRelativePosition();
            pitchPercentage = delta.x;
            rollPercentage = delta.y;
        }
        float throttleValue = throttle.getThrottleValue();
        flightPhysics.Move(rollPercentage,pitchPercentage,0f,throttle.getThrottleValue(),true);
                 
    }

    

    #endregion

    #region Private_Functions
    private void updatePlane()
    {
        
        jetBody.transform.Rotate(currentRotationPerSecond, Space.Self);

        //setspeed
        Vector3 forward = jetBody.transform.forward;
        forward = forward * currentSpeed * Time.fixedDeltaTime;
        jetBody.MovePosition((jetBody.position + forward));
        
        
        //graphics
        calculateFlightStickAdjustmentVector(delta);
        throttleObject.transform.localRotation = Quaternion.Euler(new Vector3(calculateThrottleAdjustmentAngle(throttle.getThrottleValue()),0,0));


        //jetBody.velocity = new Vector3(0, 0, currentSpeed);
    }


    #region modelUpdate

    private float calculateThrottleAdjustmentAngle(float throttleValue)
    {
        return ((-90 * throttleValue) -90); // Calculate Angle of Throttle
    }

    private void calculateFlightStickAdjustmentVector(Vector3 rotatedFlightStickVector3)
    {
        Vector3 flightStickadjustment;
        if (rotatedFlightStickVector3.x != 0 && rotatedFlightStickVector3.y == 0)  // only x axis is involved
        {
            flightStickadjustment.x = (-90 + rotatedFlightStickVector3.x);
            flightStickadjustment.y = 0;
            flightStickadjustment.z = 0;
            
        } else if (rotatedFlightStickVector3.x == 0 && rotatedFlightStickVector3.y != 0) // only y axis is involved
        {
            flightStickadjustment.x = (-90 + rotatedFlightStickVector3.y);
            flightStickadjustment.y = 90;
            flightStickadjustment.z = -90;
        }
        else // x and y axis participate 
        {
            flightStickadjustment.x = (-90 + rotatedFlightStickVector3.x);
            flightStickadjustment.y = rotatedFlightStickVector3.y;
            flightStickadjustment.z = flightStickadjustment.y * (-1);
        }
        flightStickObject.transform.localRotation = Quaternion.Euler(flightStickadjustment);
    }

    #endregion
    
    #endregion
    public void alignPlaneWithHMD()
    {
        GameObject cockpit = GameObject.Find("Cockpit");
        GameObject player = GameObject.FindWithTag("Player");
        if (cameraZeroRotation.eulerAngles != Vector3.zero)
        {
            player.transform.rotation = cameraZeroRotation;
            return;
        }
        Vector3 targetPosition = Vector3.zero - GameObject.FindWithTag("MainCamera").transform.localRotation.eulerAngles;
        targetPosition.x = 0;
        targetPosition.z = 0;
        player.transform.Rotate(targetPosition);
        cameraZeroRotation = player.transform.rotation;
    }
    
    }// End class
/*
 * 
 

*/