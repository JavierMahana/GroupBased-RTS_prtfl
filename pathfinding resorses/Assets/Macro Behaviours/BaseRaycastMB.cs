using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BaseRaycastMB : SerializedScriptableObject
{
    protected bool ObstacleToDestination(Vector2 pos, Vector2 des, float radious, int layerMask)
    {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(pos, des, layerMask);
        if (hit.collider != null)
            return true;

        Vector2 perp = Vector2.Perpendicular((des - pos).normalized);

        hit = Physics2D.Linecast(pos + perp * radious, des + perp * radious, layerMask);
        if (hit.collider != null)
            return true;

        hit = Physics2D.Linecast(pos - perp * radious, des - perp * radious, layerMask);
        if (hit.collider != null)
            return true;


        return false;
    }
}
