using UnityEngine;

public class Fonctions
{
    public static float Fract(float x)
    {
        return x - Mathf.Floor(x);
    }

    public static Vector2 Hash(Vector2 P)
    {
        Vector2 res = new Vector2();
        Vector2 k = new Vector2(0.3183099f, 0.3678794f);
       
        P.x = P.x * k.x + k.y;
        P.y = P.y * k.y + k.x;

        res.x = -1 + 2 * Fract(16 * k.x * Fract(P.x * P.y * (P.x + P.y)));
        res.y = -1 + 2 * Fract(16 * k.y * Fract(P.x * P.y * (P.x + P.y)));

        return res;
    }

    public static float QuinticRes(float x)
    {
        return x * x * x * (x * (x * 6 - 15) + 10);
    }

    public static float QuinticDRes(float x)
    {
        return 30 * x * x * (x * (x - 2) + 1);
    }

    public static float CubicRes(float x)
    {
        return x * x * (x * (-2) + 3);
    }

    public static float CubicDRes (float x)
    {
        return x * (x * (-6) + 6);
    }

    
}
