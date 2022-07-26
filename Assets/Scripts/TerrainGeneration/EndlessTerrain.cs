using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    #region Constants
    private const float scale = 20f; // scale the mesh
    private const float viewerMoveThresholdForChunkUpdate = 25f; //minimal distance the viewer has to move for the terrain to be updated
    private const float sqrViewerMoveThreshholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    #endregion
    
    #region Public
    public LODInfo[] detailLevels;
    public Transform viewer;
    public Material mapMaterial;
    public static float maxViewDst;
    public static Vector2 viewerPosition;
    #endregion

    #region Private
    private Vector2 viewerPositionOld;
    private static MapGenerator mapGenerator;
    private int chunksize;
    private int chunksVisibleInViewDistance;
    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        //make sure both lists are clear
        terrainChunksVisibleLastUpdate.Clear();
        terrainChunkDictionary.Clear();
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDistanceThreshhold;
        chunksize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDst / chunksize);
        //initial terrain generation

        for (int i = -15; i < 15; i++)
        {
            for (int j = -15; j < 15; j++)
            {
                Vector2 viewedChunkCoord = new Vector2(i, j);
                terrainChunkDictionary.Add(viewedChunkCoord,
                    new TerrainChunk(viewedChunkCoord, chunksize, detailLevels, transform, mapMaterial));
            }
        }
        updateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        //if viewer moved enough to warrant a terrain update
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThreshholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            updateVisibleChunks();
        }
    }
    #endregion
    
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
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord,
                        new TerrainChunk(viewedChunkCoord, chunksize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    #region Classes

    /// <summary>
    /// The generated terrain is seperated into chunks. These get enabled or disabled based on the distance to the viewer.
    /// </summary>
    public class TerrainChunk
    {
        private GameObject meshObject;
        private Vector2 position;
        private Bounds bounds;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;
        private LODMesh collisionLodMesh;
        private MapData mapData;
        private bool mapDataReceived;
        private int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshCollider.name = "Ground";
            meshRenderer.material = material;
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            setVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];

            // figure out which lod gets used for collider generation
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].levelOfDetail, updateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLodMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        /// <summary>
        /// Sets the Chunk visible based on Distance to viewer. Also requests the mesh for the chunk if it not already has one.
        /// </summary>
        public void updateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistanceFromNearestEdge <= maxViewDst;
                //only show if viewer is close enough
                if (visible)
                {
                    int lodIndex = 0;
                    //set the level of detail based on viewer distance
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshhold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    
                    //update mesh if the lod changed
                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    //if lod is highest possible add a collider to the mesh
                    if (lodIndex == 0)
                    {
                        if (collisionLodMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionLodMesh.mesh;
                        }
                        else if (!collisionLodMesh.hasRequestedMesh)
                        {
                            collisionLodMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                setVisible(visible);
            }
        }
        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize,
                MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            updateTerrainChunk();
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

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;
        private System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.updateCallback = updateCallback;
            this.lod = lod;
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.createMesh();
            hasMesh = true;
            updateCallback();
        }
    }

    #endregion
    
}


#region structs
[System.Serializable]
public struct LODInfo
{
    public int levelOfDetail;
    public float visibleDistanceThreshhold;
    public bool useForCollider;
}
#endregion