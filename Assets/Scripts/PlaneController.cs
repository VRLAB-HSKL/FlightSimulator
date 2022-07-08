using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

[RequireComponent(typeof(JoyStick))]
[RequireComponent(typeof(Throttle))]
public class PlaneController : MonoBehaviour
{
    #region Public_Members
    public JoyStick joyStick;
    public Throttle throttle;
    
    public float accelerationPerSecond = 20f;
    public float maxSpeedPerSec = 343f; // 1fache schallgeschwindigkeit
    #endregion

    #region Private_Members
    
    public float currentSpeed = 0f; // currentspeed
    public float speedGoal = 0f; // goal in range(0, maxspeedpersec)

    private Vector3 currentRotationPerSecond = Vector3.zero;
    private Vector3 maxRotationPerSecond = new Vector3(50, 0, 50); //x: Pitch z: Roll 

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
        }
        else
        {
            currentRotationPerSecond = Vector3.zero;
            //todo stop rotating smoothly
        }
        updatePlane();
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Pad))
        {
            //alignPlaneWithHMD();
        }

    }

    

    #endregion

    #region Private_Functions
    private void updatePlane()
    {
        //jetBody.MoveRotation(Quaternion.Euler(currentRotationPerSecond));
        jetBody.rotation *= Quaternion.Euler(currentRotationPerSecond);
        speedGoal = Mathf.Abs(speedGoal);
        
        if (speedGoal > currentSpeed)
        {
            Debug.Log(speedGoal);
            currentSpeed += accelerationPerSecond;
            if (currentSpeed > maxSpeedPerSec) currentSpeed = maxSpeedPerSec;
        }

        Vector3 forward = jetBody.transform.forward;
        forward = forward * currentSpeed * Time.fixedDeltaTime;
        jetBody.MovePosition((jetBody.position + forward));
        //jetBody.velocity = new Vector3(0, 0, currentSpeed);
    }

    private void alignPlaneWithHMD()
    {
        GameObject hmd = GameObject.FindWithTag("Player");
        GameObject cockpit = GameObject.Find("Cockpit");
        cockpit.transform.position = hmd.transform.position;
        cockpit.transform.rotation = hmd.transform.rotation;
    }
    #endregion

    
    }// End class
