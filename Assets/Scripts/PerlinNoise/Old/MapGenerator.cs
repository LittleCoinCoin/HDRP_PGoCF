using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)
public class MapGenerator : MonoBehaviour {

    public TerrainData terraindata;
    public NoiseData noisedata;
    public TextureData texturedata;

    public Material TerrainMaterial;

    public enum DrawMode {NoiseMap, Mesh};
    public DrawMode drawmode;

    public bool AutoMapUpdate;

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
       {
            GenerateMap();
       }
    }

    void OnTextureUpdated()
    {
        texturedata.ApplyToMaterial(TerrainMaterial);
    }

    public void GenerateMap()
    {
        float[,] MapNoise = new float[terraindata.MapWidth, terraindata.MapHeight];


        MapNoise = NoiseGenerator.GenerationNoiseMap(
            terraindata.MapWidth, terraindata.MapHeight,
            noisedata.noisetype, noisedata.Seed, noisedata.Octaves,
            noisedata.NoiseScale, noisedata.Persistance, noisedata.Lacunarity, noisedata.Offset, noisedata.WarpIntensity, noisedata.WarpRecursivity, noisedata.WarpSeed);

        //for (int y = 0; y < terraindata.MapHeight; y++)
        //{
        //    for (int x = 0; x < terraindata.MapWidth; x++)
        //    {
        //        float currentnoise = MapNoise[x, y];
        //    }
        //}

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawmode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromNoiseMap(MapNoise));
        }
        else if (drawmode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(MapNoise, terraindata.NoiseMultiplier, terraindata.MeshHeightCurve));
            texturedata.UpdateMeshHeights(TerrainMaterial, terraindata.Minheight, terraindata.Maxheight);
        }
    }

    void OnValidate()
    {

        if (terraindata!=null)
        {
            terraindata.OnValuesUpdated -= OnValuesUpdated;
            terraindata.OnValuesUpdated += OnValuesUpdated;
        }

        if (noisedata != null)
        {
            noisedata.OnValuesUpdated -= OnValuesUpdated;
            noisedata.OnValuesUpdated += OnValuesUpdated;
        }

        if (texturedata != null)
        {
            texturedata.OnValuesUpdated -= OnTextureUpdated;
            texturedata.OnValuesUpdated += OnTextureUpdated;
        }


    }
}
#endif

