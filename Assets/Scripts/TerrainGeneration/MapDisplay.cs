using UnityEngine;

/// <summary>
/// Displays the mesh changes during edit mode.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    
    public void DrawTexture(Texture2D texture)
    {
        //shared material is needed to see the change of texture in editor mode
        //renderer.material would work fine in game mode
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);

    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.createMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
