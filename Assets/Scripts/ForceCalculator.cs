using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ForceCalculator
{
    public const float g = 9.81f;
    
    public static float calculateGForce(Vector3 oldVelocity, Vector3 currentVelocity, float deltaTime)
    {
        float gs;
        float oldMagnitude = oldVelocity.magnitude;
        float currentMagnitude = currentVelocity.magnitude;
        //Vector3 vDelta = currentVelocity - oldVelocity;
        float v = currentMagnitude - oldMagnitude;
        gs = v / (deltaTime * g);
        return gs;
    }
}
