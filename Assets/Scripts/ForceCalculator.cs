using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ForceCalculator
{
    public const float g = 9.81f; // gravitational acceleration on earth
    
    /// <summary>
    /// Calculates the Gforce after the formula g = v / (t * 9,81)
    /// </summary>
    /// <param name="oldVelocity">Velocity at t = 0</param>
    /// <param name="currentVelocity">Velocity at t = deltaTime</param>
    /// <param name="deltaTime"></param>
    /// <returns>gforce</returns>
    public static float calculateGForce(Vector3 oldVelocity, Vector3 currentVelocity, float deltaTime)
    {
        float gs;
        float oldMagnitude = oldVelocity.magnitude;
        float currentMagnitude = currentVelocity.magnitude;
        float v = currentMagnitude - oldMagnitude;
        gs = v / (deltaTime * g);
        return gs;
    }
}
