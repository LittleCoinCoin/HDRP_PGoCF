using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture _TextureMap)
    {
        TextureRenderer.sharedMaterial.mainTexture = _TextureMap;
        TextureRenderer.transform.localScale = new Vector3(_TextureMap.width, 1, _TextureMap.height);
    }

    public void DrawMesh(MeshData _MeshData)
    {
        meshFilter.sharedMesh = _MeshData.CreateMesh();
    }
}
