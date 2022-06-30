using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class GetButtonInputLeft : MonoBehaviour
{
    int device = HTCViveControllerProfile.ViveController.leftHand.device.deviceId;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger))
        {
            Debug.Log("Left" + device + Input.mousePosition);
        }  
    }
}
