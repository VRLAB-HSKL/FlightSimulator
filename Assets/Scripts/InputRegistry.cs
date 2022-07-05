using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class InputRegistry : MonoBehaviour
{
    public static event Action OnLeftTriggerPressed;
    public static event Action OnRightTriggerPressed;
    public static event Action OnLeftTriggerReleased;
    public static event Action OnRightTriggerReleased;
    public static event Action OnUKeyPressed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger))
        {
            OnLeftTriggerPressed?.Invoke(); // Throttle sollte aufgerufen werden
        }
        

        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            OnRightTriggerPressed?.Invoke(); // Joystick müsste aufgerufen werden
        }
        
        if (ViveInput.GetPressUp(HandRole.LeftHand, ControllerButton.Trigger))
        {
            OnLeftTriggerReleased?.Invoke();
        }
        

        if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
        {
            OnRightTriggerReleased?.Invoke();
        }

    }
}
