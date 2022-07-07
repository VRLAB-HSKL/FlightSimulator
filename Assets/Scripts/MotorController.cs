using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SerialConnection))]
public class MotorController : MonoBehaviour
{
    #region CONSTANTS
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
    #endregion

    #region UNITY_LIFECYCLE
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            updateMotors();
        }
    }
    #endregion

    #region PUBLIC_FUNCTIONS

    public void updateMotors()
    {
        if (SerialConnection.stream.IsOpen)
        {
            Debug.Log("UPDATE MOTORS");
            setMotor1Rotation(motor1NewGoal);
            setMotor2Rotation(motor2NewGoal);
            setMotor3Rotation(motor3NewGoal);    
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
    #endregion
    
    #region PRIVATE_FUNCTIONS
    private void setMotor1()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.LOW);
    }

    private void resetMotor1()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR1_PORT_SET, SerialConnection.LOW);
    }
    private void setMotor2()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.LOW);
    }

    private void resetMotor2()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR2_PORT_SET, SerialConnection.LOW);
    }
    private void setMotor3()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.LOW);
    }

    private void resetMotor3()
    {
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_RESET, SerialConnection.HIGH);
        SerialConnection.setOutputOnPort(SerialConnection.MOTOR3_PORT_SET, SerialConnection.LOW);
    }
    
    #endregion
    
    
    
}
