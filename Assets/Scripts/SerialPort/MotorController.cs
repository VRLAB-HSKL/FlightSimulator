using UnityEngine;
using static SerialConnection.State;

[RequireComponent(typeof(SerialConnection))]
public class MotorController : MonoBehaviour
{
    
    #region UNITY_LIFECYCLE
    void Start()
    {
        // make sure all motors are disabled on startup
        disableMotor1();
        disableMotor2();
        disableMotor3();
    }
    #endregion

    #region Misc
    /// <summary>
    /// Checks if the Serialport of the Serialconnection component is open.
    /// </summary>
    /// <returns>Is the serialport open?</returns>
    public bool connected()
    {
        return SerialConnection.stream.IsOpen;
    }
    #endregion
    
    /// <summary>
    /// Every motor consists of 2 ports: SET and RESET
    /// The two states contradict each other, so whenever "SetMotorX()" is called the SET will be set to HIGH
    /// and RESET will be set to 0
    /// This results in 80 bit that get send over the serial port with every function call.
    /// </summary>
    #region Set Motor Functions
    public void setMotor1()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_SET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_RESET, LOW);
    }

    public void resetMotor1()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_RESET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_SET, LOW);
    }

    public void disableMotor1()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_RESET, LOW);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR1_PORT_SET, LOW);
    }
    public void setMotor2()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_SET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_RESET, LOW);
    }

    public void resetMotor2()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_RESET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_SET, LOW);
    }
    public void disableMotor2()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_RESET, LOW);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR2_PORT_SET, LOW);
    }
    public void setMotor3()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_SET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_RESET, LOW);
    }

    public void resetMotor3()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_RESET, HIGH);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_SET, LOW);
    }
    public void disableMotor3()
    {
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_RESET, LOW);
        SerialConnection.setOutputOnPin(SerialConnection.MOTOR3_PORT_SET, LOW);
    }

    public void disableAllMotors()
    {
        SerialConnection.resetAll();
    }
    #endregion
}
