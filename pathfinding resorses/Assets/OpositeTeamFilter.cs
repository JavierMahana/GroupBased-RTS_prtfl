using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Target Filter/Only Oposite Team")]
public class OpositeTeamFilter : ScriptableObject, IEntityFilter
{
    public List<IEntity> FilterEntities(AIUnit requester, List<IEntity> unfilteredEntities)
    {
        List<IEntity> filteredEntities = new List<IEntity>();
        foreach (IEntity unFiltered in unfilteredEntities)
        {
            if (unFiltered.Team != requester.Team)
            {
                filteredEntities.Add(unFiltered);
            }
        }
        return filteredEntities;
    }
}
