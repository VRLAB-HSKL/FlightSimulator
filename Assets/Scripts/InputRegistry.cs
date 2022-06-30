using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class InputRegistry : MonoBehaviour
{
    public static event Action OnLeftTriggerPressed;
    public static event Action OnRightTriggerPressed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger))
        {
            OnLeftTriggerPressed?.Invoke();
        }
        

        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            OnRightTriggerPressed?.Invoke();
        }
    }
}
