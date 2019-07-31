using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "AI/Behavior Set/Simple")]
public class SimpleBS : BehaviourSet, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;

    public Vector2 CalculateDesiredPosition(AIAgent requester, float deltaTime)
    {
        Vector2 requesterPosition = GetPostion(requester);
        Vector2 currentDesiredPosition;

        if (useStuckBehaviour)
        {
            if (requester.UseStuckBehaviour(out Vector2? sbDestination))
            {
                
                currentDesiredPosition = SteeringBehaviour.Arribe(requester, requesterPosition, sbDestination.Value, deltaTime, false, false);
            }
            else
            {
                currentDesiredPosition = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester, deltaTime);
            }

        }
        else
        {
            currentDesiredPosition = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester, deltaTime);
        }



        if (applySeparation)
        {
            currentDesiredPosition = ApplySeparation(requester, requesterPosition, currentDesiredPosition, linearSeparationWeight);
        }
        if (applyAvoidance)
        {
            if (!ignoreIfHeading)
                currentDesiredPosition = ApplyAvoidance(requester, requesterPosition, currentDesiredPosition, linearAvoidanceWeight);
            else
                currentDesiredPosition = ApplyAvoidance(requester, requesterPosition, currentDesiredPosition, linearAvoidanceWeight, ingnoreAnglesAvoidance);
        }

        return currentDesiredPosition;
    }


}
