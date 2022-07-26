using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Generates map data for a terrain chunk that is used as base for the meshes.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    #region Constants

    public const int mapChunkSize = 239;

    #endregion

    public enum DrawMode
    {
        NoiseMap, // flat black and white noise map
        ColorMap, // flat colored map
        Mesh, // generates a mesh
        FalloffMap // visualizes a falloff map (and only the falloff map)
    };

    #region Editor Variables

    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    [Range(0, 6)] public int editorLevelOfDetailPreview;
    public float noiseScale; //

    public int numberOfOctaves; // numbers of noise maps that get added up

    [Range(0, 1)] public float persistance; // factor by which the influence of octaves decreases per octave level
    public float lacunarity; // Increase of frequence per octave level

    public int seed; // seed for map generation
    public Vector2 offSet; // offSet for noise

    public bool useFalloff; // should only be water at the chunk edges?

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve; //cut off the lowest values
    public bool autoUpdate; // autoupdate in preview mode while editing

    public TerrainType[] regions;

    #endregion

    #region Private

    private float[,] falloffMap;
    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    #endregion

    #region Unity_Lifecycle

    private void Awake()
    {
        falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (numberOfOctaves < 0) numberOfOctaves = 0;
        falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    #endregion

    #region Public

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;
            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve,
                        editorLevelOfDetailPreview),
                    TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.FalloffMap:
                display.DrawTexture(
                    TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(mapChunkSize)));
                break;
        }
    }

    #endregion

    #region Generation

    /// <summary>
    /// Generates a map around the center
    /// </summary>
    /// <param name="centre"></param>
    /// <returns></returns>
    private MapData GenerateMapData(Vector2 centre)
    {
        //get noisemap
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale,
            numberOfOctaves,
            persistance, lacunarity, centre + offSet, normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                //clamp values according to falloff map
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                float currentHeight = noiseMap[x, y];
                // set the color for the current height, specified in the regions array
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    #endregion

    /*
     To improve performance Terrainchunks do not generate their meshes on the main thread.
     Instead they request a mesh/map data object from the map generator.
     The map generator generates the mesh inside a thread and then supplies the mesh back to the caller.
     */

    #region Thread Handling

    /// <summary>
    /// starts the thread for map generation.
    /// </summary>
    /// <param name="centre">Center of the map</param>
    /// <param name="callback">Method that is supposed to be called on completion</param>
    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(centre, callback); };
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Thread for map generation.
    /// </summary>
    /// <param name="centre"></param>
    /// <param name="callback"></param>
    private void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }


    /// <summary>
    /// Starts the thread for mesh generation.
    /// </summary>
    /// <param name="mapData"></param>
    /// <param name="lod">level of detail for the mesh</param>
    /// <param name="callback"></param>
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, lod, callback); };
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Thread for mesh generation.
    /// </summary>
    /// <param name="mapData"></param>
    /// <param name="lod">level of detail of the mesh</param>
    /// <param name="callback"></param>
    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData =
            MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    #endregion
}

#region Structs

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

struct MapThreadInfo<T>
{
    public readonly Action<T> callback;
    public readonly T parameter;

    public MapThreadInfo(Action<T> callback, T parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}

#endregion