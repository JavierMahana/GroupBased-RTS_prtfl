using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Creation/Spawn Filter/No filter")]
public class NoFilterSF : ScriptableObject, ISpawnFilter
{
    public bool CanSpawn(out string errorMessage)
    {
        errorMessage = "No Error";
        return true;
    }
}
