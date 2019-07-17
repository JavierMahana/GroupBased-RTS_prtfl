using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behavior Set/Stuck")]
public class StuckBS : BehaviourSet, IBehaviourSet
{
    public Vector2 CalculateDesiredPosition(AIAgent requester, float deltaTime)
    {
        Vector2 position = requester.transform.position;
        Vector2 currentDesiredPosition = SteeringBehaviour.Arribe(requester, position, requester.stuckDestination, deltaTime);


        if (applySeparation)
            currentDesiredPosition = ApplySeparation(requester, position, currentDesiredPosition, linearSeparationWeight);

        if (applyAvoidance)
        {
            if (!ignoreIfHeading)
                currentDesiredPosition = ApplyAvoidance(requester, position, currentDesiredPosition, linearAvoidanceWeight);
            else
                currentDesiredPosition = ApplyAvoidance(requester, position, currentDesiredPosition, linearAvoidanceWeight, ingnoreAnglesAvoidance);
        }

        return currentDesiredPosition;
    }
}
