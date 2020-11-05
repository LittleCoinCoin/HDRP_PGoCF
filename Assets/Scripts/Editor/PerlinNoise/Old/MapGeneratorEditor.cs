using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator MAP = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (MAP.AutoMapUpdate)
            {
                MAP.GenerateMap();
            }
        }
        if (GUILayout.Button("GeneratePerlinNoiseMap"))
        {
            MAP.GenerateMap();
        }
    }
}
