using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator 
{
    /// <summary>
    /// Generates a falloff map that has values between -1 and 1.
    /// </summary>
    /// <param name="size">Size of the noise map the falloff map is added to</param>
    /// <returns></returns>
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // if i / size is 0 or 1 x is on the edge, if it is .5 x is in the center of the map
                float x = i / (float) size;
                float y = j / (float) size;

                // put values in a range between -1 and 1
                x = x * 2 - 1;
                y = y * 2 - 1; 

                // get coord that is closest to the edge of the map
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = evaluate(value);
            }
        }

        return map;
    }

    /// <summary>
    /// Adjusts the given value after formula (value ^ a) / (value ^ a) + (b - b * value) ^ a
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static float evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
