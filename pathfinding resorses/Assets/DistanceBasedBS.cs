using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/Behavior Set/Distance Based")]
public class DistanceBasedBS : ScriptableObject, IBehaviourSet
{
    public SteeringBehaviour.Behaviour baseBehaviour;
    public SteeringBehaviour.Behaviour closeUpBehaviour;
    public float GetCloseUpWeight(AIAgent requester)
    {
        Vector2 position = requester.transform.position;
        AIUnit parent = requester.parent;
        Vector2 parentPosition = parent.transform.position;

        float parentSqrDistance = Vector2Utilities.SqrDistance(position, parentPosition);

        float maxSqrDist = Mathf.Pow(parent.data.closeUpBehaviourArea, 2);
        float totalInfluenceSqrDistance = Mathf.Pow(parent.data.closeUpBehaviourMaxIntensityArea, 2);
        if (maxSqrDist < parentSqrDistance)
            return 0;
        else if (parentSqrDistance <= totalInfluenceSqrDistance)
            return 1;

        float weight = parentSqrDistance / maxSqrDist;
        return weight;
            
    }

    public SteeringBehaviour.Behaviour localSeparation;
    public float localSeparationWieght = 0.5f;

    public Vector2 CalculateDesiredPosition(AIAgent requester)
    {
        //el close up se como todoooooo
        //el separation tien pinta que no esta haciendo nada


        Vector2 currentDesiredPostion = SteeringBehaviour.GetDesiredPosition(baseBehaviour, requester).Value;
        currentDesiredPostion = Vector2.Lerp(currentDesiredPostion,
            SteeringBehaviour.GetDesiredPosition(closeUpBehaviour, requester).Value, GetCloseUpWeight(requester));

        Vector2? localSepTemp = SteeringBehaviour.GetDesiredPosition(localSeparation, requester);
        if (localSepTemp.HasValue)
        {
            currentDesiredPostion = Vector2.Lerp(currentDesiredPostion, localSepTemp.Value, localSeparationWieght);
        }

        return currentDesiredPostion;
    }

    
}
