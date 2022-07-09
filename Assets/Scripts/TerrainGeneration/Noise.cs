using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /*
     * @param scale - rez of noise map (smaller = more noisce)
     * @param octaves - layers of overlaping noise
     * @param persistance - controls decrease in amplitude of octaves in range(0,1) (amplitude(octave n) = persistance ^ n)
     * @param lacunarity - controls the increase in frequency of octaves (frequence(octave n) = lacunarity ^ n)
     */
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight]; // map with heightvalue for every point on the map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offSetX = prng.Next(-100000, 100000) + offset.x;
            float offSetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offSetX, offSetY);
        }
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        //keep track of highest and lowest value of the noisemap
        //used for normalizing the map after generation
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapWidth / 2f;
        
        //generate noise values
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                //octave loop
                for (int i = 0; i < octaves; i++)
                {
                    //the higher the frequency, the further apart are the sample points => leading to more spikes
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; 
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }//end octave loop
                
                //update min max
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                noiseMap[x, y] = noiseHeight;

            }//end row loop
        }//end column loop

        //normalize noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);// returns percentage value where noiseMap[x,y] is on a scale between minNoiseHeight and MaxNoiseHeight
                
            }
        }

        return noiseMap;
    }
}
