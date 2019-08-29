using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


public class UnitManager : SerializedMonoBehaviour
{    
    public HashSet<AIUnit> allActiveUnits = new HashSet<AIUnit>();    
    
    private void OnUnitCreation(AIUnit unit)
    {        
        allActiveUnits.Add(unit);
        unit.OnDeath += OnUnitDeath;
    }    
    private void OnUnitDeath(IKillable unit)
    {
        Debug.Assert(unit is AIUnit);

        allActiveUnits.Remove((AIUnit)unit);
        unit.OnDeath -= OnUnitDeath;
    }

    public List<AIUnit> FindUnits(AIUnitData data, Team team)
    {
        if (allActiveUnits == null || allActiveUnits.Count == 0) return new List<AIUnit>();

        List<AIUnit> returnUnits = new List<AIUnit>();

        foreach (AIUnit unit in allActiveUnits)
        {
            if (unit.Data == data && unit.team == team)
            {
                returnUnits.Add(unit);
            }
        }

        //if (returnUnits.Count == 0) return null;
        //else 
        return returnUnits;
    }


    private void Awake()
    {
        AIUnit.OnSpawn += OnUnitCreation;
    }
    private void OnDisable()
    {
        AIUnit.OnSpawn -= OnUnitCreation;
    }
}
