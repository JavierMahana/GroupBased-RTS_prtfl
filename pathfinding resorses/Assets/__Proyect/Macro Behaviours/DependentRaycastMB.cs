using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Macro Behaviour/Dependent Raycast")]
public class DependentRaycastMB : BaseRaycastMB, IMacroBehaviour
{
    public IBehaviourSet actingBS;
    public IBehaviourSet defaultBS;
    public IBehaviourSet interferenceBS;

    public LayerMask raycastMask;

    public IBehaviourSet GetBehaviourSet(AIAgent requester)
    {
        Vector2 pos = requester.transform.position;
        Vector2 des = requester.Destination;

        if (ObstacleToDestination(pos, des,requester.data.radious, raycastMask))
        {
            return interferenceBS;
        }

        float sqrDist = Vector2Utilities.SqrDistance(pos, des);
        if (sqrDist <= Mathf.Pow(requester.entityAction.BaseData.rangeOfAction + requester.data.reachDestinationMargin, 2))
        {
            return actingBS;
        }
            
        return defaultBS;
    }

    public Vector2 GetDesiredDestination(AIAgent requester)
    {
        Vector2 position = requester.transform.position;
        AIAgentData requesterData = requester.data;
        IEntity target = requester.Target;
        if (target == null)
        {
            //Debug.LogError($"{requester} has no posible targets");
            return requester.parent.GetCohesionPosition(requester);
        }

        Vector2 targetPos = target.GameObject.transform.position;
        Vector2 directionTowardsPosition = (position - targetPos).normalized;

        float requesterRadiousOffset = requesterData.shape == Shape.CIRCULAR ? requesterData.radious :
            Vector2Utilities.GetDistanceOfSquareEdgeAndCenterFromDirection(requesterData.radious, directionTowardsPosition);

        float targetRadiousOffset = target.BodyShape == Shape.CIRCULAR ? target.Radious :
            Vector2Utilities.GetDistanceOfSquareEdgeAndCenterFromDirection(target.Radious, directionTowardsPosition);

        Vector2 destinationPoint = targetPos + directionTowardsPosition * (requesterRadiousOffset + targetRadiousOffset);
        return destinationPoint;
    }

}
