using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Utilities 
{
    public static Vector2 Normalize(Vector2 a, out float magnitude)
    {
        magnitude = a.magnitude;
        return a / magnitude;
    }

    public static float SqrDistance(Vector2 a, Vector2 b)
    {
        return Vector2.SqrMagnitude(a - b);
    }
}
