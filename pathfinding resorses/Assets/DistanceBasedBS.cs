using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/Behavior Set/Distance Based")]
public class DistanceBasedBS : BehaviourSet, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;
    public SteeringBehaviour.Behaviour closeUpBehaviour;


    public SteeringBehaviour.Behaviour localSeparation;

    public Vector2 CalculateDesiredPosition(AIAgent requester)
    {
        Vector2 requesterPosition = GetPostion(requester);

        Vector2 currentDesiredPostion = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requesterPosition, requester).Value;
        //if (requester.debug) Debug.Log($"desired antes del close up| magnitud = {(currentDesiredPostion- requesterPosition).magnitude/Time.deltaTime}|| {(currentDesiredPostion - requesterPosition) / Time.deltaTime}"); 

        currentDesiredPostion = Vector2.Lerp(currentDesiredPostion,
            SteeringBehaviour.GetDesiredPosition(closeUpBehaviour, requesterPosition, requester).Value, GetCloseUpWeight(requester));
        //if (requester.debug) Debug.Log($"desired DESPUES del close up| magnitud = {(currentDesiredPostion - requesterPosition).magnitude / Time.deltaTime}|| {(currentDesiredPostion - requesterPosition) / Time.deltaTime}");

        currentDesiredPostion = ApplySeparation(requester, requesterPosition, currentDesiredPostion);
        currentDesiredPostion = ApplyAvoidance(requester, requesterPosition, currentDesiredPostion);

        return currentDesiredPostion;
    }



    private float GetCloseUpWeight(AIAgent requester, bool linear = true)
    {
        Vector2 position = requester.transform.position;
        AIUnit parent = requester.parent;
        Vector2 cohesionPosition = parent.GetCohesionPosition(requester);

        return GetWeight(position, cohesionPosition, parent.data.closeUpBehaviourArea, parent.data.closeUpBehaviourMaxIntensityArea, linear);

        float parentSqrDistance = Vector2Utilities.SqrDistance(position, cohesionPosition);
        float sqrBehaviourAreaRadious = Mathf.Pow(parent.data.closeUpBehaviourArea, 2);
        float totalInfluenceSqrDistance = Mathf.Pow(parent.data.closeUpBehaviourMaxIntensityArea, 2);
        if (sqrBehaviourAreaRadious < parentSqrDistance)
            return 0;
        else if (parentSqrDistance <= totalInfluenceSqrDistance)
            return 1;


        float weight;
        if (linear)
            weight = 1 - (Mathf.Sqrt(parentSqrDistance) / parent.data.closeUpBehaviourArea);
        else
            weight = 1 - parentSqrDistance / sqrBehaviourAreaRadious;

         
        return weight;

    }
}
