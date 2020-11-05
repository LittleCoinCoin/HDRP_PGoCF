using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] _NoiseMap, float _HeightMultiplier, AnimationCurve _HeightCurve)
    {
        int MapWidth = _NoiseMap.GetLength(0);
        int MapHeight = _NoiseMap.GetLength(1);
        //on centre le mesh à l'écran
        float topleftX = (MapWidth - 1) / -2f;
        float topleftZ = (MapHeight - 1) / 2f;

        MeshData meshdata = new MeshData(MapWidth, MapHeight);
        
        int VertexIndex = 0;
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                meshdata.vertices[VertexIndex] = new Vector3(x+topleftX, _HeightCurve.Evaluate(_NoiseMap[x, y]) * _HeightMultiplier, y+topleftZ);
                meshdata.uvs[VertexIndex] = new Vector2(x / (float)MapWidth, y / (float)MapHeight);

                if (x < (MapWidth - 1) && y < (MapHeight - 1))
                {
                    meshdata.AddTriangle(VertexIndex, VertexIndex + MapWidth, VertexIndex + MapWidth + 1);
                    meshdata.AddTriangle(VertexIndex + MapWidth + 1, VertexIndex + 1, VertexIndex);
                }
                VertexIndex++;
            }
        }
        return meshdata;
    }
}


public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    int triangleindex;
    
    public MeshData (int _MapWidth, int _MapHeight)
    {
        vertices = new Vector3[_MapHeight * _MapWidth];
        uvs = new Vector2[_MapHeight * _MapWidth];
        triangles = new int[(_MapWidth - 1) * (_MapHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleindex] = a;
        triangles[triangleindex+1] = b;
        triangles[triangleindex+2] = c;

        triangleindex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}