using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetableFilter
{
    List<ITargetable> FilterTargets(AIUnit requester, List<ITargetable> unfilteredTargeteables);
}
