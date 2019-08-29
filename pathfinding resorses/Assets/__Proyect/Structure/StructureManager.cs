using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{    
    public HashSet<Structure> allActiveStructures = new HashSet<Structure>();
    
    private void OnStructureCreation(Structure structure)
    {
        allActiveStructures.Add(structure);
        structure.OnDeath += OnStructureDeath;
    }    
    private void OnStructureDeath(IKillable structure)
    {
        Debug.Assert(structure is Structure);

        allActiveStructures.Remove((Structure)structure);
        structure.OnDeath -= OnStructureDeath;
    }

    private void Awake()
    {
        Structure.OnSpawn += OnStructureCreation;
    }

    private void OnDisable()
    {
        Structure.OnSpawn -= OnStructureCreation;
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
