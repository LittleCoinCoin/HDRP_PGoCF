using UnityEngine;

#if (UNITY_EDITOR)
public static class NoiseGenerator
{ 
	public static float[,] GenerationNoiseMap(int MapWidth, int MapHeight, NoiseData.NoiseType noisetype, int Seed, 
        int Octaves, float NoiseScale, float persistance, float lacunarity, Vector2 offset,
        float _WarpIntensity, int _WarpRecursivity, int _WarpSeed)
    {
        float[,] NoiseMap = new float[MapWidth, MapHeight];
        //calcul de l'offset pour décaler le bruit sur x ou sur y (en vue plan)
        System.Random pnrg = new System.Random(Seed);
        Vector2[] octavesoffsets = new Vector2[Octaves];
        for (int i = 0; i< Octaves; i++)
        {
            float offsetX = pnrg.Next(-10000, 10000)+offset.x;
            float offsetY = pnrg.Next(-10000, 10000)+offset.y;
            octavesoffsets [i] = new Vector2(offsetX, offsetY);
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
                    switch (noisetype)
                    {
                        case NoiseData.NoiseType.AnalyticalDerivatives:
                        {
                            float perlinValue = Mathf.PerlinNoise(SampleX, SampleY);
                            float NoiseHeightAD = AnalyticalDerivatives(perlinValue, perlinValue, Octaves);
                            NoiseHeight += (NoiseHeightAD+perlinValue)*amplitude;
                            break;
                        }

                        case NoiseData.NoiseType.ADWarpedNoise:
                        {
                            float perlinValue = DomainWarping(SampleX, SampleY, _WarpIntensity, _WarpRecursivity, _WarpSeed);
                            float NoiseHeightAD = AnalyticalDerivatives(perlinValue, perlinValue, Octaves);
                            NoiseHeight += (NoiseHeightAD + perlinValue) * amplitude;
                            break;
                        }

                        case NoiseData.NoiseType.NormalWarpedNoise:
                        {
                            float perlinValue = DomainWarping(SampleX, SampleY, _WarpIntensity, _WarpRecursivity, _WarpSeed);
                            NoiseHeight += perlinValue * amplitude;
                            break;
                        }
                        case NoiseData.NoiseType.NormalNoise:
                        {
                            float perlinValue = Mathf.PerlinNoise(SampleX, SampleY);
                            NoiseHeight += perlinValue * amplitude;
                            break;
                        }
                    }
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
    public static float DomainWarping(float _x, float _y, float __WarpIntensity, int __WarpRecursivity, int __WarpSeed)
    {
        float xWarpSum = _x;
        float yWarpSum = _y;

        System.Random pnrg = new System.Random(__WarpSeed);
        float WarpoffsetX;
        float WarpoffsetY;

        for (int i = 0; i < __WarpRecursivity; i++)
        {
            WarpoffsetX = pnrg.Next(0, 11);
            WarpoffsetY = pnrg.Next(0, 11);
            float SampleXWarp = Mathf.PerlinNoise(xWarpSum + WarpoffsetX, yWarpSum + WarpoffsetY);

            WarpoffsetX = pnrg.Next(0, 11);
            WarpoffsetY = pnrg.Next(0, 11);
            float SampleYWarp = Mathf.PerlinNoise(xWarpSum + WarpoffsetX, yWarpSum + WarpoffsetX);

            xWarpSum = SampleXWarp;
            yWarpSum = SampleYWarp;
        }

        float perlinValue = Mathf.PerlinNoise(_x + __WarpIntensity * xWarpSum, _y + __WarpIntensity * yWarpSum);

        return perlinValue;
    }

    public static Vector3 ADNoise(float _x, float _y , string FunctionForm)
    {
        Vector3 Res = new Vector3();
        Vector2 i = new Vector2(Mathf.Floor(_x), Mathf.Floor(_y));
        Vector2 f = new Vector2(Fonctions.Fract(_x), Fonctions.Fract(_y));
        Vector2 u = new Vector2();
        Vector2 du = new Vector2();
        switch (FunctionForm)
        {
            case "Quintic":
                {
                    u.x = Fonctions.QuinticRes(_x);
                    u.y = Fonctions.QuinticRes(_y);
                    du.x = Fonctions.QuinticDRes(_x);
                    du.y = Fonctions.QuinticDRes(_y);
                    break;
                }
            case "Cubic":
                {
                    u.x = Fonctions.CubicRes(_x);
                    u.y = Fonctions.CubicRes(_y);
                    du.x = Fonctions.CubicDRes(_x);
                    du.y = Fonctions.CubicDRes(_y);
                    break;
                }
        }
        Vector2 ga = Fonctions.Hash(i + Vector2.zero);
        Vector2 gb = Fonctions.Hash(i + Vector2.right);
        Vector2 gc = Fonctions.Hash(i + Vector2.up);
        Vector2 gd = Fonctions.Hash(i + Vector2.one);

        float va = Vector2.Dot(ga, f - Vector2.zero);
        float vb = Vector2.Dot(gb, f - Vector2.right);
        float vc = Vector2.Dot(gc, f - Vector2.up);
        float vd = Vector2.Dot(gd, f - Vector2.one);

        Res.x = va + u.x * (vb - va) + u.y * (vc - va) + u.x * u.y * (va - vb - vc + vd);
        Res.y = ga.x + u.x * (gb.x - ga.x) + u.y * (gc.x - ga.x) + u.x * u.y * (ga.x - gb.x - gc.x + gd.x) + du.x * (u.y * (va - vb - vc + vd) + vb - va);
        Res.z = ga.y + u.x * (gb.y - ga.y) + u.y * (gc.y - ga.y) + u.x * u.y * (ga.y - gb.y - gc.y + gd.y) + du.y * (u.x * (va - vb - vc + vd) + vc - va);
        
        return Res;
    }

    public static float AnalyticalDerivatives(float _x, float _y, int Octaves)
    {
        float[,] m = new float[2,2];
        m[0, 0] = 0.8f;
        m[0, 1] = -0.6f;
        m[1, 0] = 0.6f;
        m[1, 1] = 0.8f;
        Vector2 p = new Vector2(_x, _y);
        float NoiseSum = 0.0f;
        float b = 1.0f;
        Vector2 d = new Vector2(0.0f, 0.0f);
        for (int i = 0; i < 15; i++)
        {
            Vector3 n = ADNoise(p.x, p.y, "Quintic");
            //Debug.Log(n);
            d.x += n.y;
            d.y += n.z;
            NoiseSum += (b * n.x) / (1 + Vector3.Dot(d, d));
            b *= 0.5f;
            p.x = 2 * (m[0, 0] * p.x + m[0, 1] * p.y);
            p.y = 2 * (m[1, 0] * p.x + m[1, 1] * p.y);
        }
        return NoiseSum;
    }
}
#endif
