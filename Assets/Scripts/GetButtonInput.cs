/*
 * Was wurde geändert?
 * 05.07.22 - 08:50 {
 * Das Grabben über den Code wurde herausgenommen, da es nicht funktioniert hat.
 * }
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace HTC.UnityPlugin.Vive
{
    
    
    public class GetButtonInput : MonoBehaviour
    {
        int device = HTCViveControllerProfile.ViveController.rightHand.device.deviceId;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            {
                Debug.Log("Rechts" + device + Input.mousePosition);
            }
            
            
        }
    }
} // end namespace