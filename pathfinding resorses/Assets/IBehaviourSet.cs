using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourSet
{

    //la idea es que con eso pueda acceder a la informaión externa 
    Vector3 GetDesiredPosition(AIAgent requester);
}
