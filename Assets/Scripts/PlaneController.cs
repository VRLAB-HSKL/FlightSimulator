using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class PlaneController : MonoBehaviour
{

    private float speed;

    private float maxSpeed;

    private float speedGoal;

    float testVelocity = 0f;
    float testVelocityRotate = 0f;

    public JoyStick joyStick;

    public Throttle throttle;
    /****** Variablen ******/
    
    private Rigidbody airplane;
    
    // Throttle - Beschleunigung
    private float throttleZero = -90f; 
    
    // Maximale Beschleunigung 
    private float accelerationMax = 20f; 


    // Joystick - Achsenverschiebung
    private float xAxis, yAxis, zAxis;

    private Rigidbody Rigidbody; 
    private Vector3 velocity;


    #region Unity_LifeCycle
    void Start()
    {
        
        Rigidbody = GetComponent<Rigidbody>();
        velocity = new Vector3(0,0f,0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        updateVectorTiltRotate(testVelocity, testVelocityRotate);
        if (throttle.tracking)
        {
            
        }

        if (joyStick.tracking)
        {
            Vector3 delta = joyStick.getRelativePosition();
            float rollPercentage = delta.y / 90;
            float pitchPercentage = delta.x / 90;
            Debug.Log($"Rollpercentage: {rollPercentage} \t PitchPercentage: {pitchPercentage}");
            updateVectorTiltRotate(pitchPercentage, rollPercentage);
            
        }

    }
    

    #endregion
    

    void updateVectorTiltRotate(float tilt, float rotate)
    {
        Vector3 degreesVector = new Vector3(tilt, 0f, rotate);
        Quaternion quart = Quaternion.Euler(degreesVector * Time.fixedDeltaTime);
        Rigidbody.MoveRotation(Rigidbody.rotation * quart);
    }
    
   /* void rotate(float degrees)
    {
        Vector3 degreesVector = new Vector3(testVelocity, 0, degrees);
        Quaternion quart = Quaternion.Euler(degreesVector * Time.fixedDeltaTime);
        rigi.MoveRotation(rigi.rotation * quart);
    }

    void tilt(float degrees)
    {
        
        Vector3 degreesVector = new Vector3(degrees, 0f, testVelocityRotate); // Drehung des Flugzeuges um die X-Achse
        Quaternion quart = Quaternion.Euler(degreesVector * Time.fixedDeltaTime);
        rigi.MoveRotation(rigi.rotation * quart);
    }
*/
    void accelerate(float schub, Vector3 zeroPosition, Vector3 newJoystickPosition)
    {
        Vector3 newPosition;
        if (schub == 0)
        {
            newPosition = zeroPosition; // Ohne Schub kann sich das Flugzeug nicht verändern -> ohne Schub würde es Schweben //TODO
        }
        else
        {
            newPosition = zeroPosition + (schub * newJoystickPosition); // Vektorielle Addition. Ein Punkt wird mit einem Vektor und einem Lambda zu einem neuen Punkt generiert.
            //cockpitAirplane.transform.Translate(newPosition * Time.deltaTime); // Smoothing des Fluges 
            airplane.velocity = newPosition;
        }
    }
    float schubStaerke(float throttleZero, float throttleNeu, float accelerationMax)
    {
        float neueSchubStaerke = 1 - (throttleNeu / throttleZero);
        neueSchubStaerke = neueSchubStaerke * accelerationMax; // Berechnung der neuen Geschwindigkeit
        return neueSchubStaerke;
    }
    
    Vector3 resetPositionPlane(float schub, Vector3 zeroPosition, Vector3 newJoystickPosition)
    {
        Vector3 newPosition;
        if (schub == 0)
        {
            newPosition = zeroPosition; // Ohne Schub kann sich das Flugzeug nicht verändern -> ohne Schub würde es Schweben //TODO
        }
        else
        {
            newPosition = zeroPosition + (schub * newJoystickPosition); // Vektorielle Addition. Ein Punkt wird mit einem Vektor und einem Lambda zu einem neuen Punkt generiert.
            //cockpitAirplane.transform.Translate(newPosition * Time.deltaTime); // Smoothing des Fluges 
            airplane.velocity = newPosition;
        }

        return newPosition;
    }
    }// End class
