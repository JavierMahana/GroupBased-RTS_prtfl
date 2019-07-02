using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/Behavior Set/Distance Based")]
public class DistanceBasedBS : ScriptableObject, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;
    public SteeringBehaviour.Behaviour closeUpBehaviour;


    public SteeringBehaviour.Behaviour localSeparation;

    

    public Vector2 CalculateDesiredPosition(AIAgent requester)
    { 

        Vector2 currentDesiredPostion = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requester).Value;
        currentDesiredPostion = Vector2.Lerp(currentDesiredPostion,
            SteeringBehaviour.GetDesiredPosition(closeUpBehaviour, requester).Value, GetCloseUpWeight(requester));


        Vector2? localSepTemp = SteeringBehaviour.Separation(requester,  (currentDesiredPostion - (Vector2)requester.transform.position).normalized, out AIAgent closestSiblin, out Vector2 directionToclosestSiblin);        
        
        float separationWeight = GetSeparationWeight(requester, closestSiblin, false);
        if (UseLocalSeparation(requester, localSepTemp, directionToclosestSiblin, currentDesiredPostion, 90, 0))
        {
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, localSepTemp.Value, separationWeight * requester.data.wieghtMultiplier);
        }
        //if (UseLocalSeparation(requester, localSepTemp, currentDesiredPostion, 75))
        //{
        //    currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, localSepTemp.Value, separationWeight * 0.3f);
        //}
        ////closestSiblin.Push(
        //    ((Vector2)closestSiblin.transform.position - (Vector2)requester.transform.position).normalized,
        //    separationWeight * closestSiblin.data.maxSpeed * Time.deltaTime * 0.2f);
        return currentDesiredPostion;
    }
    private float GetSeparationWeight(AIAgent requester, AIAgent closestSiblin, bool linear = true)
    {
        Vector2 position = requester.transform.position;
        Vector2 siblinPos = closestSiblin.transform.position;
        AIAgentData data = requester.data;
        return GetWeight(position, siblinPos, data.radious * data.separationRangeInRadious, data.radious);
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
    private float GetWeight(Vector2 position, Vector2 destination, float areaEffectDistance, float totalInfluenceDistance, bool linear = true)
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
    private bool UseLocalSeparation(AIAgent requester, Vector2? localSeparationSteering, Vector2 closestSiblinDirection, Vector2 desiredPos, int ignoreBiggerAngles, int ignoreSmallerAngles)
    {
        if (! localSeparationSteering.HasValue)
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
        float angle = Vector2.Angle(desiredDirection, closestSiblinDirection);

        if (angle < ignoreSmallerAngles || angle > ignoreBiggerAngles)
            return false;
        else
            return true;
    }
    private bool UseLocalSeparation(AIAgent requester, Vector2? localSeparationSteering, Vector2 desiredPos, int ignoreBiggerAngles, int ignoreSmallerAngles)
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
