using UnityEngine;

#if (UNITY_EDITOR)
[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public enum NoiseType { NormalNoise, NormalWarpedNoise, AnalyticalDerivatives, ADWarpedNoise }
    public NoiseType noisetype;

    public float NoiseScale;
    public int Octaves;
    [Range(0, 1)] public float Persistance;
    public float Lacunarity;
    public int Seed;
    public Vector2 Offset;

    public float WarpIntensity;
    public int WarpRecursivity;
    public int WarpSeed;

    protected override void OnValidate()
    {
        if (Lacunarity < 1)
        {
            Lacunarity = 1;
        }
        if (Octaves < 0)
        {
            Octaves = 0;
        }
        if (WarpIntensity < 0)
        {
            WarpIntensity = 0.0f;
        }
        if (WarpRecursivity < 0)
        {
            WarpRecursivity = 0;
        }
        base.OnValidate();
    }
}
#endif
