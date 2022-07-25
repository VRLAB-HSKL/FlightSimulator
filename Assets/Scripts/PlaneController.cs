
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;




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
    
    public GameObject throttleObject; // model of the throttle in the cockpit
    public GameObject flightStickObject; // model of the flightstick in the cockpit
    public AudioManager audioManager;
    
    public float motorUpdateInterval = .25f; // how many seconds pass between motor updated
    #endregion

    #region Private_Members
    private Rigidbody jetBody;

    private Quaternion startPositionFlightStick; // zero position of the flightstick model
    private Vector3 flightStickRotation; 
    private Vector2 flightStickDelta;
    private bool isFlying = false; // did then plane leave the runway
    
    private float timeSinceLastMotorUpdate = 0f;
    private Vector3 velocityLastMotorUpdate; // used to calculate the g forces between updates
    #endregion

    #region TestVariables
    private Quaternion cameraZeroRotation= Quaternion.Euler(0,0,0 );
    #endregion
    
    


    #region Unity_LifeCycle
    void Start()
    {
        jetBody = GetComponent<Rigidbody>();
        flightPhysics = GetComponent<FlightPhysics>();
        audioManager.setVolume("highSpeedSound", 0f); // no speed sound at the start
        
        
        startPositionFlightStick = joyStick.transform.rotation;
        startPositionFlightStick.x -= 90;
        
    }
    
    void FixedUpdate()
    {
        updatePlane();

        updateHotasModel();
        
        updateSound();

        updateMotionSeat();
    }

    #region colliders

    private void OnCollisionStay(Collision other)
    {
        // keep the plane still while on the runway
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

    private void OnCollisionEnter(Collision other)
    {
        //reset the scene on collision with the terrain mesh
        if (other.gameObject.name == "Ground")
        {
            audioManager.Play("crash");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    #endregion
    
    #endregion
    
    /// <summary>
    /// Extracts values from joystick and throttle. Moves the plane accordingly.
    /// </summary>
    private void updatePlane()
    {
        float pitchPercentage = 0f;
        float rollPercentage = 0f;
        if (joyStick.tracking)
        {
            flightStickDelta = joyStick.getRelativePosition();
            pitchPercentage = flightStickDelta.x;
            rollPercentage = flightStickDelta.y;
        }

        float throttleValue = throttle.getThrottleValue();
        if (throttleValue > 0)
        {
            isFlying = true;
        }
        float convertedThrottle = (throttleValue * 2) - 1;
        flightPhysics.Move(rollPercentage, pitchPercentage, 0f, convertedThrottle, true);
    }
    #region ModelUpdate
    
    /// <summary>
    /// Aligns the rotation of the throttle and flightstick with the vive controllers.
    /// </summary>
    private void updateHotasModel()
    {
        adjustFlightStickModel(flightStickDelta);
        throttleObject.transform.localRotation = Quaternion.Euler(new Vector3(calculateThrottleAdjustmentAngle(throttle.getThrottleValue()),0,0));
    }
    
    private float calculateThrottleAdjustmentAngle(float throttleValue)
    {
        return ((180 * throttleValue) -180);
    }

    /// <summary>
    /// Rotates the Flightstick model between 90 and -90 degrees in x and z axis. 
    /// </summary>
    /// <param name="rotatedFlightStickVector2">Percentage rotated in x and z axis</param>
    private void adjustFlightStickModel(Vector2 rotatedFlightStickVector2)
    {
       
        Vector2 flightStickadjustment;
        
        flightStickadjustment.x = (-90 * rotatedFlightStickVector2.x) + startPositionFlightStick.x;
        flightStickadjustment.y = (90 * rotatedFlightStickVector2.y);
        Quaternion origin = startPositionFlightStick;
        Quaternion change = Quaternion.Euler(flightStickadjustment.x, 0, flightStickadjustment.y);
        Quaternion delta = Quaternion.Inverse(change) * origin;
        flightStickObject.transform.localRotation = delta;


    }
    
    #endregion
    
    
    /// <summary>
    /// Plays sound according to the game situation.
    /// </summary>
    private void updateSound()
    {
        //if plane is still on the ground the default sound will play
        if (!isFlying)
        {
            if (!audioManager.isPlaying("idle"))
            {
                audioManager.Play("idle");    
            }
            return;
        }
        if (throttle.getThrottleValue() > flightPhysics.Throttle && flightPhysics.getCurrentSpeedInPercent() < 0.95f)
        {
            audioManager.Play("accelerate");
        }
        if(!audioManager.isPlaying("highSpeedSound")) audioManager.Play("highSpeedSound");
        
        audioManager.setVolume("highSpeedSound", flightPhysics.getCurrentSpeedInPercent());
    }

    
    
    #region Motorupdates
    /// <summary>
    /// Updates the arduino pins in set intervals.
    /// </summary>
    private void updateMotionSeat()
    {
        if (!m_MotorController.connected()) return;
        timeSinceLastMotorUpdate += Time.deltaTime;
        if (timeSinceLastMotorUpdate >= motorUpdateInterval)
        {
            
            timeSinceLastMotorUpdate = 0f;
            m_MotorController.disableAllMotors();
            updateForwardGForce();
            updateRotationMotors();
        }
        
    }
    /// <summary>
    /// Calculates the gforce based on the delta of the current velocity and the velocity of last update. Sets/Resets motor 1
    /// if the gforce reaches a certain threshhold. 
    /// </summary>
    private void updateForwardGForce()
    {
        
        Vector3 currentVelocity = jetBody.velocity;
        float g = ForceCalculator.calculateGForce(velocityLastMotorUpdate, currentVelocity, motorUpdateInterval);

        velocityLastMotorUpdate = currentVelocity;
        //Debug.Log($"Current Gs: {g}");

        if (g > 0.3f)
        {
            m_MotorController.setMotor1();
        }
        else if (g < -0.3f)
        {
            m_MotorController.resetMotor1();
        }
        
        
    }

    /// <summary>
    /// Used set motor2/3 according to the rotation of the jetbody.
    /// </summary>
    private void updateRotationMotors()
    {
        Vector3 currentRotation = jetBody.gameObject.transform.localRotation.eulerAngles;
        float zAxis = currentRotation.z;
        Debug.Log(zAxis);
        if (zAxis > 25 && zAxis < 115)
        {
            //left
            m_MotorController.setMotor2();
            m_MotorController.resetMotor3();
        }

        if (zAxis < 335 && zAxis > 245)
        {
            //right
            m_MotorController.setMotor3();
            m_MotorController.resetMotor2();
        }

        if (zAxis > 115 && zAxis < 245)
        {
            //topdown
            m_MotorController.setMotor2();
            m_MotorController.setMotor3();
        }
    }
    #endregion
    
    
   
    /// <summary>
    /// Used to align the rotation of the hmd with the plane in edit mode.
    /// </summary>
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
    
   
}// End class

