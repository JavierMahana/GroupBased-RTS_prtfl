using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable 
{
    //event KillableEvent Onspawn;
    event KillableEvent OnDeath;
}
public delegate void KillableEvent(IKillable killable);
