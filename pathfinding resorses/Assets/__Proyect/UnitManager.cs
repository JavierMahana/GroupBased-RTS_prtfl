using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitManager : Singleton<UnitManager>
{
    public HashSet<AIUnit> allActiveUnits;
    public UnitDataToUnit unitDictionary;

    //debe subscribirse a un evento estatico al crearse una unidad.
    private void OnUnitCreation(AIUnit unit)
    {        
        allActiveUnits.Add(unit);
    }
    //se subscribe a este evento al ser creada una unidad
    private void OnUnitDeath(AIUnit unit)
    {
        allActiveUnits.Remove(unit);
    }

    public List<AIUnit> FindUnits(AIUnitData data, Team team)
    {
        List<AIUnit> returnUnits = new List<AIUnit>();

        foreach (AIUnit unit in allActiveUnits)
        {
            if (unit.data == data && unit.Team == team)
            {
                returnUnits.Add(unit);
            }
        }

        if (returnUnits.Count == 0) return null;
        else return returnUnits;
    }


    private void OnEnable()
    {
        AIUnit.OnDeath += OnUnitDeath;
        AIUnit.OnSpawn += OnUnitCreation;
    }
    private void OnDisable()
    {
        AIUnit.OnDeath -= OnUnitDeath;
        AIUnit.OnSpawn -= OnUnitCreation;
    }
    private void Start()
    {
        AIUnit[] au = FindObjectsOfType<AIUnit>();
        foreach (AIUnit unit in au)
        {
            OnUnitCreation(unit);
        }
    }
}
