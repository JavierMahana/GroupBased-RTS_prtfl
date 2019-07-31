using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName ="AI/Behavior Set/Distance Based")]
public class DistanceBasedBS : BehaviourSet, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;
    public SteeringBehaviour.Behaviour closeUpBehaviour;


    public bool closeUpLinearWeight = true;
    public float closeUpBehaviourArea = 1.6f;
    public float closeUpBehaviourMaxIntensityArea = 0.4f;

    public Vector2 CalculateDesiredPosition(AIAgent requester, float deltaTime)
    {
        Vector2 requesterPosition = GetPostion(requester);
        Vector2 currentDesiredPosition;

        if (useStuckBehaviour)
        {
            if (requester.UseStuckBehaviour(out Vector2? sbDestination))
            {
                currentDesiredPosition = SteeringBehaviour.Arribe(requester, requesterPosition, sbDestination.Value,deltaTime, false, false);
            }
            else
            {
                currentDesiredPosition = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester, deltaTime);
                currentDesiredPosition = Vector2.Lerp(currentDesiredPosition,
                    SteeringBehaviour.GetDesiredPosition(closeUpBehaviour, requesterPosition, requester, deltaTime), GetCloseUpWeight(requester, closeUpLinearWeight));
            }

        }
        else
        {
            currentDesiredPosition = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester, deltaTime);
            currentDesiredPosition = Vector2.Lerp(currentDesiredPosition,
                SteeringBehaviour.GetDesiredPosition(closeUpBehaviour, requesterPosition, requester, deltaTime), GetCloseUpWeight(requester, closeUpLinearWeight));
        }



        if (applySeparation)
            currentDesiredPosition = ApplySeparation(requester, requesterPosition, currentDesiredPosition, linearSeparationWeight);
        if (applyAvoidance)
        {
            if (!ignoreIfHeading)
                currentDesiredPosition = ApplyAvoidance(requester, requesterPosition, currentDesiredPosition, linearAvoidanceWeight);
            else
                currentDesiredPosition = ApplyAvoidance(requester, requesterPosition, currentDesiredPosition, linearAvoidanceWeight, ingnoreAnglesAvoidance);
        }
        return currentDesiredPosition;
    }



    private float GetCloseUpWeight(AIAgent requester, bool linear = true)
    {
        Vector2 position = requester.transform.position;
        AIUnit parent = requester.parent;
        Vector2 cohesionPosition = parent.GetCohesionPosition(requester);

        return GetWeight(position, cohesionPosition, closeUpBehaviourArea, closeUpBehaviourMaxIntensityArea, linear);
    }
}
