using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//la estructura de datos no esta muy buena.
//es medio un desastre
//lo que voy a hacer es desordenarla aun más ahora.
//lograr que la UI funcione y luego la ordeno.
//sae
[CreateAssetMenu (menuName ="AI/Unit Data")]
public class AIUnitData : SerializedScriptableObject
{
    public const float MB_UPDATE_TIME = 0.5f;//hacer private constante luego de probar un tiempo bueno

    [Required]
    [AssetsOnly]
    public AIAgent childPrefab;

    [Required]
    public DisplayableData displayebleData;

    public ITargetableFilter targetFilter;
    public FormationData formationData;
    public UnitSize maxSize = UnitSize.STANDART;
    public LayerMask detectableLayers;
    public LayerMask obstacleLayerMask = 1<<9;
    public float detectionRadious = 2f;
}

public enum UnitSize
{
    MASSIVE = 1,
    GRAND = 2,
    BIG = 4,
    STANDART = 9,
    //SMALL = 16 not suported yet
}
