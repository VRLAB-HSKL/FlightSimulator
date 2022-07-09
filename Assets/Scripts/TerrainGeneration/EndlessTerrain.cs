using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
   public const float maxViewDst = 450;
   public Transform viewer;

   public Material mapMaterial;
   private static MapGenerator mapGenerator;
   public static Vector2 viewerPosition;

   private int chunksize;
   private int chunksVisibleInViewDistance;
   private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
   private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
   private void Start()
   {
      mapGenerator = FindObjectOfType<MapGenerator>();
      chunksize = MapGenerator.mapChunkSize - 1;
      chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDst / chunksize);
   }

   private void Update()
   {
      viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
      updateVisibleChunks();
   }

   void updateVisibleChunks()
   {
      foreach (var terrainChunk in terrainChunksVisibleLastUpdate)
      {
         terrainChunk.setVisible(false);
      }
      terrainChunksVisibleLastUpdate.Clear();
      int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunksize);
      int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunksize);

      for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
      {
         for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
         {
            Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
            if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
            {
               terrainChunkDictionary[viewedChunkCoord].updateTerrainChunk();
               if (terrainChunkDictionary[viewedChunkCoord].isVisible())
               {
                  terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
               }
            }
            else
            {
               terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunksize, transform, mapMaterial));
            }
         }
      }

   
   }
   public class TerrainChunk
   {
      private GameObject meshObject;
      private Vector2 position;
      private Bounds bounds;
      private MeshRenderer meshRenderer;
      private MeshFilter meshFilter;
      public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
      {
         position = coord * size;
         bounds = new Bounds(position, Vector2.one * size);
         Vector3 positionV3 = new Vector3(position.x, 0, position.y);
         
         meshObject = new GameObject("Terrain Chunk");
         meshRenderer = meshObject.AddComponent<MeshRenderer>();
         meshFilter = meshObject.AddComponent<MeshFilter>();
         meshRenderer.material = material;
         meshObject.transform.position = positionV3;
         meshObject.transform.parent = parent;
         setVisible(false);
         
         mapGenerator.RequestMapData(OnMapDataReceived);
      }
      
      void OnMapDataReceived(MapData mapData)
      {
         mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
      }

      void OnMeshDataReceived(MeshData meshData)
      {
         meshFilter.mesh = meshData.createMesh();
      }

      //enable or disable meshobject based on viewdistance
      public void updateTerrainChunk()
      {
         float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
         bool visible = viewerDistanceFromNearestEdge <= maxViewDst;
         setVisible(visible);
      }

      public void setVisible(bool visible)
      {
         meshObject.SetActive(visible);
      }

      public bool isVisible()
      {
         return meshObject.activeSelf;
      }
   }
}
