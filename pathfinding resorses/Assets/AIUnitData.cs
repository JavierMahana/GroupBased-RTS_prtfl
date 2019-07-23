using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName ="AI/Unit Data")]
public class AIUnitData : SerializedScriptableObject
{
    public float MB_UPDATE_TIME = 5f;//hacer private constante luego de probar un tiempo bueno
    
    public IEntityFilter targetFilter;
    public FormationData formationData;
    public UnitType type;
    public LayerMask detectableLayers;
    public float detectionRadious = 2f;
    public float radious = 0.5f;

}

public enum UnitType
{
    MASSIVE = 1,
    GRAND = 2,
    BIG = 4,
    STANDART = 9,
    SMALL = 16
}
