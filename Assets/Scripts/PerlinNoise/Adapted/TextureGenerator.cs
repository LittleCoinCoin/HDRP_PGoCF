using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap (int _MapWidth, int _MapHeight, Color[] _ColorMap)
    {
        Texture2D texture = new Texture2D(_MapWidth, _MapHeight);
        //definit des bordures franches sur la texture
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(_ColorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromNoiseMap (float[,] _NoiseMap)
    {
        int MapWidth = _NoiseMap.GetLength(0);
        int MapHeight = _NoiseMap.GetLength(1);
        Color[] MapColor = new Color[MapWidth * MapHeight];
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                MapColor[y * MapWidth + x] = Color.Lerp(Color.black, Color.white, _NoiseMap[x, y]);
            }
        }
        return TextureFromColorMap(MapWidth, MapHeight, MapColor);
    }
}
