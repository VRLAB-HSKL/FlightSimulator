using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
<<<<<<< Updated upstream
    private float speed;

    private float maxSpeed;

    private float speedGoal;
=======
    
    /****** Variablen ******/
    
    GameObject cockpitAirplane = GameObject.Find("Cockpit");
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
            newPlaneOrientation(newJoystickPosition); // Dreht das Flugzeug in die gewünschte Richtung
            newPosition = zeroPosition + (schub * newJoystickPosition); // Vektorielle Addition. Ein Punkt wird mit einem Vektor und einem Lambda zu einem neuen Punkt generiert.
            cockpitAirplane.transform.Translate(newPosition * Time.deltaTime); // Smoothing des Fluges 
            airplaine.velocity = newPosition;
            
        }

        return newPosition;
    }

    void newPlaneOrientation(Vector3 joyStickposition)
    {
        cockpitAirplane.transform.Rotate(joyStickposition); // Rotation des Cockpits
    }
>>>>>>> Stashed changes
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // resetPositionPlane()
    }

    void rotate(float degrees)
    {
        
    }

    void tilt(float degrees)
    {
        
    }

    void accelerate(float goal)
    {
        
    }
    


}
