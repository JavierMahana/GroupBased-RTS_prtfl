using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behavior Set/Act")]
public class ActingBS : BehaviourSet, IBehaviourSet
{
    public Vector2 CalculateDesiredPosition(AIAgent requester, float deltaTime)
    {
        return GetPostion(requester);
    }

}
