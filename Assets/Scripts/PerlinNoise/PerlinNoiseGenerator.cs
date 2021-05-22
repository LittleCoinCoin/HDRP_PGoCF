using UnityEngine;

public static class PerlinNoiseGenerator
{
    public static float[,] GenerationNoiseMap(int MapWidth, int MapHeight, int Seed,
        int Octaves, float NoiseScale, float persistance, float lacunarity, Vector2 offset, Vector2 offset_random)
    {
        float[,] NoiseMap = new float[MapWidth, MapHeight];
        //calcul de l'offset pour décaler le bruit sur x ou sur y (en vue plan)
        System.Random pnrg = new System.Random(Seed);
        Vector2[] octavesoffsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++)
        {
            float offsetX = pnrg.Next(-10000, 10000) + offset.x + Random.Range(-offset_random.x, offset_random.x);
            float offsetY = pnrg.Next(-10000, 10000) + offset.y + Random.Range(-offset_random.y, offset_random.y);
            octavesoffsets[i] = new Vector2(offsetX, offsetY);
        }

        //Initialisation des max et min NoiseHeight pour la normalisation du bruit
        float MaxNoiseHeight = float.MinValue;
        float MinNoiseHeight = float.MaxValue;

        if (NoiseScale <= 0)//evite une erreur de division par zéro ligne 20 et 21
            NoiseScale = 0.0001f;

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float NoiseHeight = 0;
                for (int i = 0; i < Octaves; i++)
                {
                    float SampleX = (x / NoiseScale) * frequency + octavesoffsets[i].x;
                    float SampleY = (y / NoiseScale) * frequency + octavesoffsets[i].y;
                    //

                    float perlinValue = Mathf.PerlinNoise(SampleX, SampleY);
                    NoiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //Recherche du max et min pour normalisation
                if (NoiseHeight > MaxNoiseHeight)
                {
                    MaxNoiseHeight = NoiseHeight;
                }
                else if (NoiseHeight < MinNoiseHeight)
                {
                    MinNoiseHeight = NoiseHeight;
                }
                NoiseMap[x, y] = NoiseHeight;
            }
        }
        //Normalisation
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                NoiseMap[x, y] = Mathf.InverseLerp(MinNoiseHeight, MaxNoiseHeight, NoiseMap[x, y]);
            }
        }
        return NoiseMap;
    }
}
