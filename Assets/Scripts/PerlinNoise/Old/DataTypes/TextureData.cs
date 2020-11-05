using UnityEngine;
using System.Linq;

#if (UNITY_EDITOR)
[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] Layers;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial (Material _Material)
    {
        _Material.SetInt("_LayerCount", Layers.Length);
        _Material.SetFloatArray("_TerrainStartColor", Layers.Select(x => x.TerrainStartHeight).ToArray());
        _Material.SetFloatArray("_TerrainBlend", Layers.Select(x => x.TerrainBlend).ToArray());
        _Material.SetFloatArray("_TerrainColorStrength", Layers.Select(x => x.ColorStrength).ToArray());
        _Material.SetFloatArray("_TerrainTextureScale", Layers.Select(x => x.TextureScale).ToArray());

        Texture2DArray texturesArray = GenerateTextureArray(Layers.Select(x => x.texture).ToArray());
        _Material.SetTexture("baseTextures", texturesArray);

        _Material.SetColorArray("_TerrainColor", Layers.Select(x => x.TerrainColor).ToArray());

        UpdateMeshHeights(_Material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights (Material _Material, float _MinHeight, float _MaxHeight)
    {
        savedMinHeight = _MinHeight;
        savedMaxHeight = _MaxHeight;

        _Material.SetFloat("_MinHeigh", _MinHeight);
        _Material.SetFloat("_MaxHeight", _MaxHeight);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public float TextureScale;

        public Color TerrainColor;
        [Range(0, 1)]
        public float ColorStrength;
        [Range(0, 1)]
        public float TerrainStartHeight;
        [Range(0, 1)]
        public float TerrainBlend;
    }
}
#endif
