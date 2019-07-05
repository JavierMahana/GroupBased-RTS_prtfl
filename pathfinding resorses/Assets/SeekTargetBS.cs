using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekTargetBS : BehaviourSet, IBehaviourSet
{
    SteeringBehaviour.Behaviour seekBehaviour;

    public Vector2 CalculateDesiredPosition(AIAgent requester)
    {
        Vector2 requesterPosition = GetPostion(requester);

        Vector2 currentDesiredPostion = SteeringBehaviour.GetDesiredPosition(seekBehaviour, requesterPosition, requester).Value;
        

        currentDesiredPostion = ApplySeparation(requester, requesterPosition, currentDesiredPostion);
        currentDesiredPostion = ApplyAvoidance(requester, requesterPosition, currentDesiredPostion);

        return currentDesiredPostion;
    }

}
