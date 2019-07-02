using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/Unit Data")]
public class AIUnitData : ScriptableObject
{
    public FormationData formationData;
    public UnitType type;
    public float radious = 0.5f;

    public float closeUpBehaviourArea = 2f;
    public float closeUpBehaviourMaxIntensityArea = 0.1f;
}

public enum UnitType
{
    MASSIVE = 1,
    GRAND = 2,
    BIG = 4,
    STANDART = 9,
    SMALL = 16
}
