using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Provides a single method for the Mes generation out of a heightmap.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Generates a Mesh out of the given heightmap
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="heightMapMultiplier">Used to scale the mesh</param>
    /// <param name="_heightCurve"></param>
    /// <param name="levelOfDetail">The level of detail of the generated Mesh.
    /// 0 = one vertex per unit
    /// 1 = one vertex every two units etc.</param>
    /// <returns>The generated mesh</returns>
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMapMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        
        int borderedSize = heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * meshSimplificationIncrement;
        int meshSizeUnsimplified = borderedSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;
        
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        
        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        vertexIndicesMap = generateVertexIndexMap(borderedSize, meshSimplificationIncrement, vertexIndicesMap);

        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine);
        
        // generate vertices
        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                
                int vertexIndex = vertexIndicesMap[x, y];
                //clamp height with the heightcurve
                float height = heightCurve.Evaluate(heightMap[x, y]);
                //scale with heightmultiplier
                height  *= heightMapMultiplier;
                
                //calculate uv
                Vector2 vertexUV = new Vector2((x - meshSimplificationIncrement) / (float) meshSize, (y - meshSimplificationIncrement) / (float) meshSize);
                
                //calculate vertex position
                float posX = topLeftX + vertexUV.x * meshSizeUnsimplified; // uv.x = x / meshsize => uv.x * meshsize = vertex position
                float posY = topLeftZ - vertexUV.y * meshSizeUnsimplified;
                Vector3 vertexPosition = new Vector3(posX, height, posY);

                meshData.addVertex(vertexPosition, vertexUV, vertexIndex);

                //if vertex is not a border vertex add triangles for the vertex
                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
                    meshData.addTriangle(a, d, c);
                    meshData.addTriangle(d, a, b);
                }
                vertexIndex++;
            }
        }
        meshData.BakeNormals();
        return meshData;
    }

    /// <summary>
    /// Generates a index map. If Index[x,y] is a border vertex aka on the edge of the map, Indexmap[x,y] = -1 else 0
    /// </summary>
    /// <param name="size"></param>
    /// <param name="meshSimplificationIncrement">Amount of vertices skipped per step</param>
    /// <param name="vertexIndicesMap"></param>
    /// <returns>VertexIndicesMap</returns>
    private static int[,] generateVertexIndexMap(int size, int meshSimplificationIncrement, int[,] vertexIndicesMap)
    {
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;
        for (int y = 0; y < size; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < size; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == size - 1 || x == 0 || x == size - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        return vertexIndicesMap;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    private Vector3[] bakedNormals;

    private Vector3[] borderVertices;
    private int[] borderTriangles;

    private int triangleIndex;
    private int borderTriangleIndex;

    public MeshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        borderVertices = new Vector3[verticesPerLine * 4 + 4];
        borderTriangles = new int[6 * 4 * verticesPerLine];
    }

    public void addVertex(Vector3 vertexPosition, Vector2 vertexUV, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            borderVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = vertexUV;
        }
    }

    public void addTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    //custom method is required to fix the lightning gap between meshes
    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            if (vertexIndexA >= 0)
                vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0)
                vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0)
                vertexNormals[vertexIndexC] += triangleNormal;
        }


        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = bakedNormals;
        return mesh;
    }
}