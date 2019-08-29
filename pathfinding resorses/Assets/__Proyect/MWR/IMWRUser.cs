using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMWRUser 
{
    IKillable KillableEntity { get; }
    GameObject GameObject { get; }
    MovementWorldRepresentation MWR { get; set; }
    Vector2Int CurrentCoordintates { get; set; }
}
