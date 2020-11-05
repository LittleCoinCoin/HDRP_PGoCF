using UnityEngine;

#if (UNITY_EDITOR)
[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public int MapWidth;
    public int MapHeight;
    public float NoiseMultiplier;
    public AnimationCurve MeshHeightCurve;

    void Onvalidate()
    {
        if (MapWidth < 1)
        {
            MapWidth = 1;
        }
        if (MapHeight < 1)
        {
            MapHeight = 1;
        }

        base.OnValidate();
    }

    public float Minheight
    {
        get
        {
            return NoiseMultiplier * MeshHeightCurve.Evaluate(0);
        }
    }

    public float Maxheight
    {
        get
        {
            return NoiseMultiplier * MeshHeightCurve.Evaluate(1);
        }
    }
}
#endif