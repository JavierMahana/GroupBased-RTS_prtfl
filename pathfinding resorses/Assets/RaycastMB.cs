using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName ="AI/Macro Behaviour/Raycast")]
public class RaycastMB : SerializedScriptableObject, IMacroBehaviour
{
    public IBehaviourSet defaultBS;
    public IBehaviourSet interferenceBS;
    public IBehaviourSet idleBS;

    public LayerMask raycastMask;

    public IBehaviourSet GetBehaviourSet(AIAgent requester)
    {
        Vector2 pos = requester.transform.position;
        Vector2 des = requester.Destination;

        if (Vector2Utilities.SqrDistance(pos, des) <= Mathf.Pow(requester.parent.data.radious, 2) && !requester.parent.Moving)
        {
            return idleBS;
        }

        RaycastHit2D hit;
        hit = Physics2D.Linecast(pos, des, raycastMask);
        if (hit.collider != null)
            return interferenceBS;

        Vector2 perp = Vector2.Perpendicular((des - pos).normalized);
        float radious = requester.data.radious;

        hit = Physics2D.Linecast(pos + perp * radious, des + perp * radious, raycastMask);
        if (hit.collider != null)
            return interferenceBS;

        hit = Physics2D.Linecast(pos - perp * radious, des - perp * radious, raycastMask);
        if (hit.collider != null)
            return interferenceBS;

        return defaultBS;
    }

    public Vector2 GetDesiredDestination(AIAgent requester)
    {
        return requester.parent.GetCohesionPosition(requester);
    }
}
