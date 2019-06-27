using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourSet
{
    Vector2 CalculateDesiredPosition(AIAgent requester);
    
}
