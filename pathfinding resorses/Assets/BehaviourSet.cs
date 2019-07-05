using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourSet : ScriptableObject
{

    protected Vector2 ApplyAvoidance(AIAgent requester, Vector2 requesterPosition, Vector2 currentDesiredPostion)
    {
        Vector2? avoidanceTemp = SteeringBehaviour.ObstacleAvoidance(requester, requesterPosition,
                    (currentDesiredPostion - requesterPosition).normalized, out AIAgent avoidanceObj);
        if (avoidanceTemp != null)
        {
            float avoidanceWeight = GetAvoidanceWeight(requester, requesterPosition, avoidanceObj.transform.position);
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, avoidanceTemp.Value, avoidanceWeight * requester.data.aviodanceWieghtMultiplier);
        }

        return currentDesiredPostion;
    }

    protected Vector2 ApplySeparation(AIAgent requester, Vector2 requesterPosition, Vector2 currentDesiredPostion)
    {
        Vector2? localSepTemp = SteeringBehaviour.Separation(requester, requesterPosition,
            (currentDesiredPostion - requesterPosition).normalized, out AIAgent closestSiblin, out Vector2 directionToclosestSiblin);

        if (UseLocalSeparation(requester, localSepTemp, directionToclosestSiblin, currentDesiredPostion, requester.data.separationIgnoreBiggerAngles, requester.data.separationIgnoreSmallerAngles))
        {
            //currentDesiredPostion = currentDesiredPostion + (localSepTemp.Value - (Vector2)requester.transform.position) * separationWeight * requester.data.wieghtMultiplier;
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, localSepTemp.Value,
                GetSeparationWeight(requester, closestSiblin, false) * requester.data.separationWieghtMultiplier);
        }

        return currentDesiredPostion;
    }
    public Vector2 GetPostion(AIAgent requester)
    {
        bool hasRB = requester.body != null ? true : false;
        return hasRB ? requester.body.position : (Vector2)requester.transform.position;
    }

    public float GetAvoidanceWeight(AIAgent requester, Vector2 requesterPos, Vector2 aviodanceObjPos, bool linear = true)
    {
        return GetWeight(requesterPos, aviodanceObjPos, requester.data.aviodanceMaxRange, requester.data.aviodanceMaxInfluenceRadious);
    }
    public float GetSeparationWeight(AIAgent requester, AIAgent closestSiblin, bool linear = true)
    {
        Vector2 position = requester.transform.position;
        Vector2 siblinPos = closestSiblin.transform.position;
        AIAgentData data = requester.data;
        return GetWeight(position, siblinPos, data.radious * data.separationRangeInRadious, data.radious, linear);
    }
    public float GetWeight(Vector2 position, Vector2 destination, float areaEffectDistance, float totalInfluenceDistance, bool linear = true)
    {
        float sqrDistance = Vector2Utilities.SqrDistance(position, destination);
        float sqrAreaEfectDistance = Mathf.Pow(areaEffectDistance, 2);
        float sqrTotalInfluenceDistance = Mathf.Pow(totalInfluenceDistance, 2);
        if (sqrAreaEfectDistance < sqrDistance)
            return 0;
        else if (sqrDistance <= sqrTotalInfluenceDistance)
            return 1;


        float weight;
        if (linear)
            weight = 1 - ((Mathf.Sqrt(sqrDistance) - totalInfluenceDistance) / (areaEffectDistance - totalInfluenceDistance));
        else
            weight = 1 - (sqrDistance - sqrTotalInfluenceDistance) / (sqrAreaEfectDistance - sqrTotalInfluenceDistance);


        return weight;
    }
    public bool UseLocalSeparation(AIAgent requester, Vector2? localSeparationSteering, Vector2 closestSiblinDirection, Vector2 desiredPos, int ignoreBiggerAngles, int ignoreSmallerAngles)
    {
        if (!localSeparationSteering.HasValue)
            return false;
        
        Vector2 position = requester.transform.position;
        Vector2 desiredMovement = desiredPos - position;
        float desiredMovmentMagnitude = desiredMovement.magnitude;
        if (desiredMovmentMagnitude < Mathf.Epsilon)
        {
            return true;
        }
        Vector2 desiredDirection = desiredMovement / desiredMovmentMagnitude;
        float angle = Vector2.Angle(desiredDirection, closestSiblinDirection);

        if (angle < ignoreSmallerAngles || angle > ignoreBiggerAngles)
            return false;
        else
            return true;
    }
    public bool UseLocalSeparation(AIAgent requester, Vector2? localSeparationSteering, Vector2 desiredPos, int ignoreBiggerAngles, int ignoreSmallerAngles)
    {
        if (!localSeparationSteering.HasValue)
        {
            return false;
        }
        Vector2 position = requester.transform.position;
        Vector2 desiredMovement = desiredPos - position;
        float desiredMovmentMagnitude = desiredMovement.magnitude;
        if (desiredMovmentMagnitude < Mathf.Epsilon)
        {
            return true;
        }
        Vector2 desiredDirection = desiredMovement / desiredMovmentMagnitude;
        Vector2 separationDesMovDirection = (localSeparationSteering.Value - position).normalized;
        float angle = Vector2.Angle(desiredDirection, separationDesMovDirection);

        if (angle < ignoreSmallerAngles || angle > ignoreBiggerAngles)
            return false;
        else
            return true;
    }

}
