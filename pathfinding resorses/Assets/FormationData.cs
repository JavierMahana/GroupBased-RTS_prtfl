using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="AI/Formation Data")]
public class FormationData : SerializedScriptableObject
{
    public Dictionary<Vector2Int, Vector2[]> FormationOffsetData;
}
