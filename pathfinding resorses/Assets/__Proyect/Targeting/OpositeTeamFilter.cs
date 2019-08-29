using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Target Filter/Only Oposite Team")]
public class OpositeTeamFilter : ScriptableObject, ITargetableFilter
{
    public List<ITargetable> FilterTargets(AIUnit requester, List<ITargetable> unfilteredTargeteables)
    {
        List<ITargetable> filteredTargetables = new List<ITargetable>();
        foreach (ITargetable unFiltered in unfilteredTargeteables)
        {
            if (unFiltered.Team != requester.team)
            {
                filteredTargetables.Add(unFiltered);
            }
        }
        return filteredTargetables;
    }
}
