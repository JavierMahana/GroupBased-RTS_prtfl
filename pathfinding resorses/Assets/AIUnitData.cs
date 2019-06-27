using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/Unit Data")]
public class AIUnitData : ScriptableObject
{
    public float closeUpBehaviourArea = 2f;
    public float closeUpBehaviourMaxIntensityArea = 0.1f;
}
