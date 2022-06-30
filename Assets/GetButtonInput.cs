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

    
        
        // Variables
        public float distToPickup;
        public LayerMask pickupLayer;
        private Rigidbody holdingTarget; 
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, distToPickup, pickupLayer);
                if (colliders.Length > 0)
                {
                    holdingTarget = colliders[0].transform.root.GetComponent<Rigidbody>();
                }
                else
                {
                    holdingTarget = null;
                }
                
                
            }
            else
            {
                if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
                {
                    //adjust velocity to move to hand
                    holdingTarget.velocity =
                        (transform.position - holdingTarget.transform.position) / Time.fixedDeltaTime;
                    
                }
            }
            Debug.Log("Rechts" + device + Input.mousePosition);
        }
    }
} // end namespace