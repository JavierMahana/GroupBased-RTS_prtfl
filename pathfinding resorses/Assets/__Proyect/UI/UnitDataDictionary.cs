using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class UnitDataDictionary<T> : SerializedScriptableObject
{    
    public Dictionary<AIUnitData, T> dictionary;
}
