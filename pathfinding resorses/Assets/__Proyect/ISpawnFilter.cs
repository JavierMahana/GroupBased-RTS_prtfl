using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnFilter 
{
    bool CanSpawn(out string errorMessage);
}
