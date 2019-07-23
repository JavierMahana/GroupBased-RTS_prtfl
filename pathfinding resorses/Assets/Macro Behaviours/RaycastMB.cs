using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName ="AI/Macro Behaviour/Raycast")]
public class RaycastMB : BaseRaycastMB, IMacroBehaviour
{
    public IBehaviourSet defaultBS;
    public IBehaviourSet interferenceBS;
    public IBehaviourSet idleBS;

    public LayerMask raycastMask;

    public IBehaviourSet GetBehaviourSet(AIAgent requester)
    {
        Vector2 pos = requester.transform.position;
        Vector2 des = requester.Destination;

        if (ObstacleToDestination(pos, des, requester.data.radious, raycastMask))
        {
            return interferenceBS;
        }
        else if (Vector2Utilities.SqrDistance(pos, des) <= Mathf.Pow(requester.data.idleBSRange, 2) && !requester.parent.Moving)
        {
            
            if (requester.debug) Debug.Log($"returning parkingBS");
            return idleBS;
        }

        return defaultBS;
    }

    public Vector2 GetDesiredDestination(AIAgent requester)
    {
        return requester.parent.GetCohesionPosition(requester);
    }
}
