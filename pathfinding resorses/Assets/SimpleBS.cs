using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behavior Set/Simple")]
public class SimpleBS : BehaviourSet, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;
    public SteeringBehaviour.Behaviour separation;

    public Vector2 CalculateDesiredPosition(AIAgent requester)
    {
        Vector2 requesterPosition = GetPostion(requester);

        Vector2 currentDesiredPostion = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester).Value;

        currentDesiredPostion = ApplyAvoidance(requester, requesterPosition, currentDesiredPostion);

        currentDesiredPostion = ApplyAvoidance(requester, requesterPosition, currentDesiredPostion);

        return currentDesiredPostion;
    }


}
