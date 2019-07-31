using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Spawner : MonoBehaviour
{
    public Vector2 spawningPosition;
    public ISpawnFilter unitCreationFilter;
    public ISpawnFilter agentCreationFilter;
    public UnitDataToPrefab converter;
    public AIUnitData id;
    public Team team;


    //requiero hacer algo visual.
    //el cual identifica todas las unidades que comparten equipo y "id" para poder tenerlas como seleccionables 

    //public bool SpawnNewAgent(AIUnit parentUnit = null)
    //{
    //    if (parentUnit == null)
    //    {
    //        if (!SpawnNewUnit(out parentUnit, out string errorMessage))
    //        {
    //            Debug.Log(errorMessage);//acá debería haber un mensaje real en el juego
    //            return false;
    //        }               
    //    }

    //    if (SpawnAgentOnUnit(parentUnit))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}
    private bool SpawnAgentOnUnit(AIUnit unit)
    {
        if ((int)unit.data.maxSize <= unit.children.Count)
        {
            Debug.LogError("can't create another agent in this unit. Max count reached");
            return false;
        }

        if (!agentCreationFilter.CanSpawn(out string errorMessage))
        {
            Debug.Log(errorMessage);
            return false;
        }
        else
        {
            LeanPool.Spawn(unit.data.childPrefab, spawningPosition, Quaternion.identity);
            return true;
        }
    }
    //private bool SpawnNewUnit(out AIUnit newUnit, out string errorMessage)
    //{
    //    if (unitCreationFilter.CanSpawn(out errorMessage))
    //    {
    //        GameObject prefav = converter.dataToPrefabDictionary[id];
    //        AIUnit unitPrefav = prefav.GetComponent<AIUnit>();
    //        Debug.Assert(unitPrefav != null);

    //        newUnit = LeanPool.Spawn(unitPrefav, spawningPosition, Quaternion.identity);
    //        return true;
    //    }
    //    else
    //    {
    //        newUnit = null;
    //        return false;
    //    }
        
    //}
}
