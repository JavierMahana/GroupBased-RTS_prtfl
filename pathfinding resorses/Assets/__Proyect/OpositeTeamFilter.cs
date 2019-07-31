using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Target Filter/Only Oposite Team")]
public class OpositeTeamFilter : ScriptableObject, IEntityFilter
{
    public List<Entity> FilterEntities(AIUnit requester, List<Entity> unfilteredEntities)
    {
        List<Entity> filteredEntities = new List<Entity>();
        foreach (Entity unFiltered in unfilteredEntities)
        {
            if (unFiltered.Team != requester.Team)
            {
                filteredEntities.Add(unFiltered);
            }
        }
        return filteredEntities;
    }
}
