using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{    
    public HashSet<Structure> allActiveStructures = new HashSet<Structure>();
    
    private void OnStructureCreation(Structure structure)
    {
        allActiveStructures.Add(structure);
    }    
    private void OnStructureDeath(Structure structure)
    {
        allActiveStructures.Remove(structure);
    }

    private void Awake()
    {
        Structure.OnDeath += OnStructureDeath;
        Structure.OnSpawn += OnStructureCreation;
    }

    private void OnDisable()
    {
        Structure.OnDeath -= OnStructureDeath;
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
