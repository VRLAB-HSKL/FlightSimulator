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
    
    /****** Variablen ******/
    
    private Rigidbody airplaine;
    
    // Throttle - Beschleunigung
    private float throttleZero = -90f; // -90° sind theoretisch 0% Schub. 0° sind 100% Schub positiv und -180° sind 100% Schub negativ
    
    // Maximale Beschleunigung 
    private float accelerationMax = 20f; // 20 units per second - pos & neg 


    // Joystick - Achsenverschiebung
    private float xAxis, yAxis, zAxis;
    
    


    /****** Methoden ******/

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
         //   cockpitAirplane.transform.Translate(newPosition * Time.deltaTime); // Smoothing des Fluges 
            airplaine.velocity = newPosition;
            
        }

        return newPosition;
    }

    private Rigidbody rigi; // Test if this will work
    private Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
        rigi = GetComponent<Rigidbody>();
        velocity = new Vector3(0,0f,0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            testVelocity += 5;
        }else if (Input.GetKey(KeyCode.KeypadMinus)){
            testVelocity -= 5;
        }else if(Input.GetKey(KeyCode.O))
        {
            testVelocityRotate += 5;
        }else if (Input.GetKey(KeyCode.P))
        {
            testVelocityRotate -= 5;
        }
        updateVectorTiltRotate(testVelocity, testVelocityRotate);    
        
        
        // rotate
        /*
        if (Input.GetKey(KeyCode.O))
            testVelocityRotate += 5;
        else if (Input.GetKey(KeyCode.P))
            testVelocityRotate -= 5;
        rotate(testVelocityRotate);
        */
        
    }

    void updateVectorTiltRotate(float tilt, float rotate)
    {
        Vector3 degreesVector = new Vector3(tilt, 0f, rotate);
        Quaternion quart = Quaternion.Euler(degreesVector * Time.fixedDeltaTime);
        rigi.MoveRotation(rigi.rotation * quart);
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
            airplaine.velocity = newPosition;
            
        }
    }
    }// End class
