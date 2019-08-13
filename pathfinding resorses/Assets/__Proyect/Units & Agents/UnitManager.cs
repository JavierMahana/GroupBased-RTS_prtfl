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
    }    
    private void OnUnitDeath(AIUnit unit)
    {
        allActiveUnits.Remove(unit);
    }

    public List<AIUnit> FindUnits(AIUnitData data, Team team)
    {
        if (allActiveUnits == null || allActiveUnits.Count == 0) return new List<AIUnit>();

        List<AIUnit> returnUnits = new List<AIUnit>();

        foreach (AIUnit unit in allActiveUnits)
        {
            if (unit.data == data && unit.team == team)
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
        AIUnit.OnDeath += OnUnitDeath;
        AIUnit.OnSpawn += OnUnitCreation;
    }
    private void OnDisable()
    {
        AIUnit.OnDeath -= OnUnitDeath;
        AIUnit.OnSpawn -= OnUnitCreation;
    }

    //private void SetUpVariablesInNeeded()
    //{
    //    if (mwrManager == null)
    //    {
    //        mwrManager = GetComponent<MWRManager>();
    //        if (mwrManager == null)
    //        {
    //            mwrManager = FindObjectOfType<MWRManager>();
    //            Debug.Assert(mwrManager != null, "must put a MWRManager!");
    //        }
    //    }
    //}
}
