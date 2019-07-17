using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "AI/Agent Data")]
public class AIAgentData : SerializedScriptableObject
{
    public Team team;//no debe estar acá.
    public float radious = 0.15f;
    public Shape shape = Shape.CIRCULAR;
    public float maxSpeed = 1.2f;
    public float rangeOfAction = 0.007f;
    public float BS_UPDATE_TIME = 0.2F; //hacer private constante luego de probar un tiempo bueno

    #region move out of here variables
    public IBehaviourSet actionBehaviour;
       

    public float movementDeltaUpdateTime = 1f;
    public float stuckSqrDistanceMargin = 0.001f;
    public float stuckMovmentDeltaTreshold = 0.05f;
    public float stuckDesinationOffset = 0.5f;

    [TabGroup("Avoidance Values")]
    [Range(0, 1)]
    public float aviodanceWieghtMultiplier = 0.7f;
    [TabGroup("Avoidance Values")]
    public float aviodanceRange = 0.7f;
    [TabGroup("Avoidance Values")]
    public float aviodanceMaxInfluenceRadious = 0.1f;

    [TabGroup("Separation Values")]
    [Range(0, 1)]
    public float separationWieghtMultiplier = 0.75f;
    [TabGroup("Separation Values")]
    public int separationIgnoreBiggerAngles = 90;
    [TabGroup("Separation Values")]
    public int separationIgnoreSmallerAngles = 0;
    [TabGroup("Separation Values")]
    public float separationRangeInRadious = 2.15f;

    [TabGroup("Arribe Values")]
    public float slowDownStartRadious = 0.5f;
    [TabGroup("Arribe Values")]
    public float stopRadious = 0.005f;
    #endregion
}
