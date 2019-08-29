using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//no implementada. Pero luego estaría muy bueno
public interface ISpawnFilter 
{
    bool CanSpawn(out string errorMessage);
}
