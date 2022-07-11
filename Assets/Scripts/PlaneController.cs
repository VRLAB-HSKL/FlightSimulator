using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


[RequireComponent(typeof(JoyStick))]
[RequireComponent(typeof(Throttle))]
public class PlaneController : MonoBehaviour
{
    #region Public_Members
    public JoyStick joyStick;
    public Throttle throttle;
    
    public float accelerationPerSecond = 20f;
    public float maxSpeedPerSec = 343f; // 1fache schallgeschwindigkeit

    public GameObject throttleObject;
    public GameObject flightStickObject;
    

    public AudioSource audioSource;
    public AudioListener audioListener;
    #endregion

    #region Private_Members
    
    public float currentSpeed = 0f; // currentspeed
    public float speedGoal = 0f; // goal in range(0, maxspeedpersec)

    private Vector3 currentRotationPerSecond = Vector3.zero;
    private Vector3 maxRotationPerSecond = new Vector3(50, 0, 50); //x: Pitch z: Roll 

    private Rigidbody jetBody;

    private Vector3 flightStickRotation;
    private Vector3 delta;
    #endregion

    #region TestVariables

    public bool aligned = false;
    private Quaternion cameraZeroRotation= Quaternion.Euler(0,0,0 );

    #endregion
    
    


    #region Unity_LifeCycle
    void Start()
    {
        jetBody = GetComponent<Rigidbody>();
        audioSource.GetComponent<AudioSource>();

    }

    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        alignPlaneWithHMD();
        audioSource.Play();

        if (throttle.tracking)
        {
            speedGoal = maxSpeedPerSec * throttle.getThrottleValue();
            
            
        }

        if (joyStick.tracking)
        {
            delta = joyStick.getRelativePosition();
            float pitchPercentage = delta.x / 90;
            float rollPercentage = delta.y / 90;
            
            
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
        updatePlane();
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.PadTouch))
        {
            alignPlaneWithHMD();
        }

    }

    

    #endregion

    #region Private_Functions
    private void updatePlane()
    {
        //jetBody.MoveRotation(Quaternion.Euler(currentRotationPerSecond));
        jetBody.rotation *= Quaternion.Euler(currentRotationPerSecond);
        //flightStickObject.transform.rotation = Quaternion.Euler(currentRotationPerSecond);
        calculateFlightStickAdjustmentVector(delta);
     
        
        speedGoal = Mathf.Abs(speedGoal);
        
        if (speedGoal > currentSpeed)
        {
            Debug.Log(speedGoal);
            currentSpeed += accelerationPerSecond;
            if (currentSpeed > maxSpeedPerSec) currentSpeed = maxSpeedPerSec;
            
            // throttleAdjustment
            throttleObject.transform.rotation = Quaternion.Euler(new Vector3(calculateThrottleAdjustmentAngle(throttle.getThrottleValue()),0,0)); // adjust angle
        }

        Vector3 forward = jetBody.transform.forward;
        forward = forward * currentSpeed * Time.fixedDeltaTime;
       
        
        
        jetBody.MovePosition((jetBody.position + forward));
        
        //jetBody.velocity = new Vector3(0, 0, currentSpeed);
    }

    private void alignPlaneWithHMD()
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
        aligned = true;
    }
    
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
        flightStickObject.transform.rotation = Quaternion.Euler(flightStickadjustment);
    }
    
    
    #endregion

    
    }// End class
