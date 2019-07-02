using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Agent Data")]
public class AIAgentData : ScriptableObject
{
    public float radious = 0.5f;
    public Shape shape = Shape.CIRCULAR;
    public float maxSpeed = 1;

    #region move out of here variables
    [Range(0, 1)]
    public float wieghtMultiplier;


    public float separationRangeInRadious = 3;
    public float pushRangeInRadious = 2;

    public float slowDownStartRadious = 0.5f;
    public float stopRadious = 0.1f;
    #endregion
}
