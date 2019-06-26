using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentData : ScriptableObject
{
    public float radious = 0.5f;
    public float maxSpeed = 1;

    #region move out of here variables
    public float separationRangeInRadious = 3;

    public float slowDownStartRadious = 0.5f;
    public float stopRadious = 0.1f;
    #endregion
}
