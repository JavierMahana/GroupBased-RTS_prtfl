using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityFilter 
{
    List<Entity> FilterEntities(AIUnit requester, List<Entity> unfilteredEntities);
}
