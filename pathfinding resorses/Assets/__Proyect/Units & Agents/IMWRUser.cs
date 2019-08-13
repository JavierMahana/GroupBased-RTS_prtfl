using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMWRUser 
{
    GameObject GameObject { get; }
    MovementWorldRepresentation MWR { get; set; }
    Vector2Int CurrentCoordintates { get; set; }
}
