using UnityEngine;
public interface IMacroBehaviour 
{
    IBehaviourSet GetBehaviourSet (AIAgent requester);
    Vector2 GetDesiredDestination(AIAgent requester);

}
