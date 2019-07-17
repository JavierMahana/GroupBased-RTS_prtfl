using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityFilter 
{
    List<IEntity> FilterEntities(AIUnit requester, List<IEntity> unfilteredEntities);
}
