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
    
    public FlightPhysics flightPhysics;
    public MotorController m_MotorController;
    
    public GameObject throttleObject;
    public GameObject flightStickObject;
    
    public float motorUpdateInterval = .25f;
    
    
    #endregion

    #region Private_Members
    private Rigidbody jetBody;
    
    
    private Vector3 currentRotationPerSecond = Vector3.zero;
    private Vector3 flightStickRotation;
    private Vector2 delta;
    
    private float timeSinceLastMotorUpdate = 0f;
    #endregion

    #region TestVariables
    
    private Quaternion cameraZeroRotation= Quaternion.Euler(0,0,0 );
    private Vector3 velocityLastMotorUpdate;

    #endregion
    
    


    #region Unity_LifeCycle
    void Start()
    {
        jetBody = GetComponent<Rigidbody>();
        flightPhysics = GetComponent<FlightPhysics>();
        FindObjectOfType<AudioManager>().Play("StartWithoutEnginePower");
    }
    
    void FixedUpdate()
    {
        
        updatePlane();

        updateHotasModel();

        updateMotionSeat();
        
    }

    private void updatePlane()
    {
        float pitchPercentage = 0f;
        float rollPercentage = 0f;
        if (joyStick.tracking)
        {
            delta = joyStick.getRelativePosition();
            pitchPercentage = delta.x;
            rollPercentage = delta.y;
        }

        float throttleValue = throttle.getThrottleValue();
        float convertedThrottle = (throttleValue * 2) - 1;
        flightPhysics.Move(rollPercentage, pitchPercentage, 0f, convertedThrottle, true);
    }

    

    private void updateMotionSeat()
    {
        timeSinceLastMotorUpdate += Time.deltaTime;
        if (timeSinceLastMotorUpdate >= motorUpdateInterval)
        {
            timeSinceLastMotorUpdate = 0f;
            
            updateForwardGForce();
            updateRotationMotors();
        }
        
    }

    private void updateRotationMotors()
    {
        Vector3 currentRotation = jetBody.rotation.eulerAngles;
        float zAxis = currentRotation.z;
        m_MotorController.disableMotor2();
        m_MotorController.disableMotor3();

        if (zAxis > 45 && zAxis < 135)
        {
            //left
            m_MotorController.setMotor2();
            m_MotorController.resetMotor3();
        }

        if (zAxis < -45 && zAxis > -135)
        {
            //right
            m_MotorController.setMotor3();
            m_MotorController.resetMotor2();
        }

        if (zAxis > 135 || zAxis < -135)
        {
            //topdown
            m_MotorController.setMotor2();
            m_MotorController.setMotor3();
        }

        
    }

    private void updateForwardGForce()
    {
        
        Vector3 currentVelocity = jetBody.velocity;
        float g = ForceCalculator.calculateGForce(velocityLastMotorUpdate, currentVelocity, motorUpdateInterval);

        velocityLastMotorUpdate = currentVelocity;
        Debug.Log($"Current Gs: {g}");

        if (g > 0.3f)
        {
            m_MotorController.setMotor1();
            //play sound accelerate
        }
        else if (g < -0.3f)
        {
            m_MotorController.resetMotor1();
            //play sound decelerate
        }
        else
        {
            m_MotorController.disableMotor1();
            //play default
        }
        
    }

    #endregion

    #region Private_Functions
    private void updateHotasModel()
    {
        adjustFlightStickModel(delta);
        
        throttleObject.transform.localRotation = Quaternion.Euler(new Vector3(calculateThrottleAdjustmentAngle(throttle.getThrottleValue()),0,0));
    }


    #region modelUpdate

    private float calculateThrottleAdjustmentAngle(float throttleValue)
    {
        return ((-90 * throttleValue) -90); // Calculate Angle of Throttle
    }

    private void adjustFlightStickModel(Vector3 rotatedFlightStickVector3)
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

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<AudioManager>().Play("Crash");
    }

    public void alignPlaneWithHMD()
    {
        
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
    
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.name == "runway1")
        {
            if (throttle.getThrottleValue() == 0)
            {
                jetBody.angularDrag = 0f;
                jetBody.drag = 0f;
                jetBody.velocity = Vector3.zero;
                jetBody.angularVelocity = Vector3.zero;
            }
            
        }
    }


    
}// End class

