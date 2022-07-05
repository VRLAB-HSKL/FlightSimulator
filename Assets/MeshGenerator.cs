
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int xSize = 20;
    public int zSize = 20;
    private Mesh mesh;
    private Vector3[] vertices;

    private int[] triangles;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        updateMesh();
    }

    private void updateMesh()
    {
        mesh.Clear();
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void CreateShape()
    {
        //generate vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; // one quad needs 4 vertices, x = 1, y = 1 ==> (x+1) * (y + 1) == 4
        int i = 0;
        for (int z = 0; z < zSize + 1; z++)
        {
            for (int x = 0; x < xSize + 1; x++)
            {
                float y = Mathf.PerlinNoise(x* .5f, z*.5f) * 2f;
                vertices[i] = new Vector3(x,y,z);
                i++;
            }
        }
        
        //generate triangles
        triangles = new int[xSize * zSize * 6]; //3 points per triangle , 2 triangles per quad
        int vert = 0; // current lower left vertice of the quad
        int tris = 0; // current triangle
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                //first triangle
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                //second triangle
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++; // shift lower left triangle one point to the right
                tris += 6;
            }

            vert++;// skip the most outer vertice to prevent connecting the left edge to the right edge
        }
       
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
