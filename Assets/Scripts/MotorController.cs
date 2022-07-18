using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SerialConnection))]
public class MotorController : MonoBehaviour
{
    #region CONSTANTS

    public float timeBetweenUpdates = .25f;
    #endregion

    #region TEST

    public float motor1NewGoal;
    public float motor2NewGoal;
    public float motor3NewGoal;
    
    #endregion

    #region PRIVATE
    private float motor1_degrees_rotated = 0f; 
    
    private float motor2_degrees_rotated = 0f;
    
    private float motor3_degrees_rotated = 0f;

    private float motor1_goal = 0f;
    
    private float motor2_goal = 0f;
    
    private float motor3_goal = 0f;

    private float timeSinceLastMotorUpdate = 0f;
    #endregion

    #region UNITY_LIFECYCLE
    void Start()
    {
        
    }
    void Update()
    {
        /*
         * 
        timeSinceLastMotorUpdate += Time.deltaTime;
        if (timeSinceLastMotorUpdate >= timeBetweenUpdates)
        {
            updateMotors();
            timeSinceLastMotorUpdate = 0f;
        }
         */
    }
    #endregion

    #region PUBLIC_FUNCTIONS

    public void updateMotors()
    {
        if (SerialConnection.stream.IsOpen)
        {
           
        }
        
    }
    public void setMotor1Rotation(float degrees)
    {
        if (degrees > motor1_degrees_rotated)
        {
            resetMotor1();
        }

        if (degrees < motor1_degrees_rotated)
        {
            setMotor1();
        }

        motor1_degrees_rotated = degrees;
    }
    
    public void setMotor2Rotation(float degrees)
    {
        if (degrees > motor2_degrees_rotated)
        {
            resetMotor2();
        }

        if (degrees < motor2_degrees_rotated)
        {
            setMotor2();
        }
        motor2_degrees_rotated = degrees;
    }
    
    public void setMotor3Rotation(float degrees)
    {
        if (degrees > motor3_degrees_rotated)
        {
            resetMotor3();
        }

        if (degrees < motor3_degrees_rotated)
        {
            setMotor3();
        }
        motor3_degrees_rotated = degrees;
    }

    public bool connected()
    {
        return SerialConnection.stream.IsOpen;
    }
    #endregion
    
    #region PRIVATE_FUNCTIONS
    public void setMotor1()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.LOW);
    }

    public void resetMotor1()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.LOW);
    }

    public void disableMotor1()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.LOW);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.LOW);
    }
    public void setMotor2()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.LOW);
    }

    public void resetMotor2()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.LOW);
    }
    public void disableMotor2()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.LOW);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.LOW);
    }
    public void setMotor3()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.LOW);
    }

    public void resetMotor3()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.LOW);
    }
    public void disableMotor3()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.LOW);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.LOW);
    }
    
    #endregion
    
    
    
}
