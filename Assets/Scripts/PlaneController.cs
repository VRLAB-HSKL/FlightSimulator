using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
[RequireComponent(typeof(JoyStick))]
[RequireComponent(typeof(Throttle))]
public class PlaneController : MonoBehaviour
{
    #region Public_Members
    public JoyStick joyStick;
    public Throttle throttle;
    
    public float accelerationPerSecond = 20f;
    private float maxSpeedPerSec = 343f; // 1fache schallgeschwindigkeit
    #endregion

    #region Private_Members

    private float currentSpeed = 0f; // currentspeed
    private float speedGoal = 0f; // goal in range(0, maxspeedpersec)

    private Vector3 currentRotationPerSecond = Vector3.zero;
    private Vector3 maxRotationPerSecond = new Vector3(20, 0, 360); //x: Pitch z: Roll 

    private Rigidbody jetBody;
    
    #endregion
    
    


    #region Unity_LifeCycle
    void Start()
    {
        
        jetBody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (throttle.tracking)
        {
            speedGoal = maxSpeedPerSec * throttle.getThrottleValue();
        }

        if (joyStick.tracking)
        {
            Vector3 delta = joyStick.getRelativePosition();
            float pitchPercentage = delta.x / 90;
            float rollPercentage = delta.y / 90;
            
            currentRotationPerSecond = new Vector3(
                maxRotationPerSecond.x * pitchPercentage * Time.deltaTime,
                0,
                maxRotationPerSecond.z * rollPercentage * Time.deltaTime);
            Debug.Log(currentRotationPerSecond);
        }
        else
        {
            currentRotationPerSecond = Vector3.zero;
            //todo stop rotating smoothly
        }
        updatePlane();

    }

    

    #endregion

    #region Private_Functions
    private void updatePlane()
    {
        //jetBody.MoveRotation(Quaternion.Euler(currentRotationPerSecond));
        jetBody.rotation *= Quaternion.Euler(currentRotationPerSecond);
        jetBody.velocity = new Vector3(0f, 0f, currentSpeed);
        if (speedGoal > Mathf.Abs(currentSpeed) + accelerationPerSecond)
        {
            currentSpeed += accelerationPerSecond;
            if (currentSpeed > maxSpeedPerSec) currentSpeed = maxSpeedPerSec;
        }

        jetBody.velocity = new Vector3(0, 0, currentSpeed);
    }
    

    #endregion

    
    }// End class
