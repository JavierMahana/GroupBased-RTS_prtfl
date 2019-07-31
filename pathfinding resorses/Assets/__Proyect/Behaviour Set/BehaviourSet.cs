using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BehaviourSet : ScriptableObject
{
    public bool applySeparation = true;
    [ShowIf("applySeparation")]
    public bool linearSeparationWeight = true;

    public bool applyAvoidance = true;
    [ShowIf("applyAvoidance")]
    public bool linearAvoidanceWeight = true;
    [ShowIf("applyAvoidance")]
    public bool perpendicular = true;
    [ShowIf("applyAvoidance")]
    public bool ignoreIfHeading = false;
    private bool showAngle { get { return applyAvoidance && ignoreIfHeading; } }
    [ShowIf("showAngle")]
    public int ingnoreAnglesAvoidance = 15;

    public bool useStuckBehaviour = false;


    protected Vector2 ApplyAvoidance(AIAgent requester, Vector2 requesterPosition, Vector2 currentDesiredPostion, bool linearWeight)
    {
        Vector2? avoidanceTemp;
        AIAgent avoidanceObj;

        if (perpendicular) avoidanceTemp = SteeringBehaviour.ObstacleAvoidance(requester, requesterPosition,
                (currentDesiredPostion - requesterPosition).normalized, out avoidanceObj);

        else avoidanceTemp = SteeringBehaviour.ObstacleAvoidance(requester, requesterPosition, out avoidanceObj);

        if (avoidanceTemp != null)
        {
            float avoidanceWeight = GetAvoidanceWeight(requester, requesterPosition, avoidanceObj.transform.position, linearWeight);
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, avoidanceTemp.Value, avoidanceWeight * requester.data.aviodanceWieghtMultiplier);
        }
        return currentDesiredPostion;

    }
    /// <summary>
    /// the requester must have an target for this to work as intended
    /// </summary>
    protected Vector2 ApplyAvoidance(AIAgent requester, Vector2 requesterPosition, Vector2 currentDesiredPostion, bool linearWeight, int anglesRangeToIgnore)
    {
        Vector2 des = requester.Destination;
        Vector2? avoidanceTemp;
        AIAgent avoidanceObj;


        if (perpendicular) avoidanceTemp = SteeringBehaviour.ObstacleAvoidance(requester, requesterPosition,
                    (currentDesiredPostion - requesterPosition).normalized, out avoidanceObj);

        else avoidanceTemp = SteeringBehaviour.ObstacleAvoidance(requester, requesterPosition, out avoidanceObj);



        if (UseObstacleAvoidance(requester, avoidanceTemp, avoidanceObj, des, anglesRangeToIgnore))
        {
            float avoidanceWeight = GetAvoidanceWeight(requester, requesterPosition, avoidanceObj.transform.position, linearWeight);
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, avoidanceTemp.Value, avoidanceWeight * requester.data.aviodanceWieghtMultiplier);
        }
        return currentDesiredPostion;
    }
    protected Vector2 ApplySeparation(AIAgent requester, Vector2 requesterPosition, Vector2 currentDesiredPostion, bool linearWeight)
    {
        Vector2? localSepTemp = SteeringBehaviour.Separation(requester, requesterPosition,
            (currentDesiredPostion - requesterPosition).normalized, out AIAgent closestSiblin, out Vector2 directionToclosestSiblin);

        if (UseLocalSeparation(requester, localSepTemp, directionToclosestSiblin, currentDesiredPostion, requester.data.separationIgnoreBiggerAngles, requester.data.separationIgnoreSmallerAngles))
        {
            float multiplier = requester.data.separationWieghtMultiplier;
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, localSepTemp.Value,
                GetSeparationWeight(requester, closestSiblin, linearWeight) * multiplier);
        }

        return currentDesiredPostion;
    }

    protected Vector2 GetPostion(AIAgent requester)
    {
        bool hasRB = requester.body != null ? true : false;
        return hasRB ? requester.body.position : (Vector2)requester.transform.position;
    }

    protected float GetAvoidanceWeight(AIAgent requester, Vector2 requesterPos, Vector2 aviodanceObjPos, bool linear)
    {
        return GetWeight(requesterPos, aviodanceObjPos, requester.data.aviodanceRange, requester.data.aviodanceMaxInfluenceRadious, linear);
    }
    protected float GetSeparationWeight(AIAgent requester, AIAgent closestSiblin, bool linear)
    {
        Vector2 position = requester.transform.position;
        Vector2 siblinPos = closestSiblin.transform.position;
        AIAgentData data = requester.data;
        return GetWeight(position, siblinPos, data.radious * data.separationRangeInRadious, data.radious, linear);
    }
    protected float GetWeight(Vector2 position, Vector2 destination, float areaEffectDistance, float totalInfluenceDistance, bool linear)
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
    protected bool UseLocalSeparation(AIAgent requester, Vector2? localSeparationSteering, Vector2 closestSiblinDirection, Vector2 desiredPos, int ignoreBiggerAngles, int ignoreSmallerAngles)
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
    protected bool UseObstacleAvoidance(AIAgent requester, Vector2? avoidanceSteering, AIAgent avoidanceObj, Vector2 requesterDestination, int ignoreSmallerAngles)
    {
        if (!avoidanceSteering.HasValue)
            return false;

        Vector2 position = requester.transform.position;
        Vector2 desiredDirection = (requesterDestination - position).normalized;
        Vector2 directionTowardsAvoidanceObj = ((Vector2)avoidanceObj.transform.position - position).normalized;

        float angle = Vector2.Angle(desiredDirection, directionTowardsAvoidanceObj);

        if (angle < ignoreSmallerAngles)
            return false;
        else
            return true;
    }
}
