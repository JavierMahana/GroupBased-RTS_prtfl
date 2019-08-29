using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Utilities 
{    
    public static Vector2 VectorPerpendicularToDesired(Vector2 desiredNormalized, Vector2 referenceVectorNormalized)
    {
        float angle = Vector2.SignedAngle(desiredNormalized, referenceVectorNormalized);
        if (angle > 0)
        {
            return -Vector2.Perpendicular(desiredNormalized);
        }
        else
        {
            return Vector2.Perpendicular(desiredNormalized);
        }
    }

    public static float GetDistanceOfSquareEdgeAndCenterFromDirection(float squareRadious, Vector2 lineDirectionFromSquareCenter)
    {
        float xMag = Mathf.Abs(lineDirectionFromSquareCenter.x);
        float yMag = Mathf.Abs(lineDirectionFromSquareCenter.y);

        if (xMag == yMag)
            return Mathf.Sqrt(Mathf.Pow(lineDirectionFromSquareCenter.x, 2) * 2);

        float angle;
        if (xMag > yMag)
        {
            //cos --> h = r/cos
            //el angulo esta en el eje x.
            angle = Mathf.Atan2(lineDirectionFromSquareCenter.y, lineDirectionFromSquareCenter.x);
        }
        else
        {
            //sen --> h = r/cos(pero los angulos son distintos)
            //el angulo esta en el eje y.
            angle = Mathf.Atan2(lineDirectionFromSquareCenter.x, lineDirectionFromSquareCenter.y);
        }
        return squareRadious / Mathf.Abs(Mathf.Cos(angle));
    }

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
