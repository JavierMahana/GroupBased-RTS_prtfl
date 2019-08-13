using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MWRManager : MonoBehaviour
{    
    public AstarPath pathfinder;

    public MovementWorldRepresentation CurrentMWR
    {
        get
        {
            if (currentMWR == null)
            {
                CreateMWR();
            }
            return currentMWR;
        }
        set
        {
            currentMWR = value;
        }
    }
    private MovementWorldRepresentation currentMWR;

    private void Start()
    {
        SetUpVariablesInNeeded();

        CreateMWR();
        SubscribeToEvents();
    }
    private void OnDisable()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        AIUnit.OnSpawn += OnUserSpawn;
        AIUnit.OnDeath += OnUserDeath;

        Structure.OnSpawn += OnUserSpawn;
        Structure.OnDeath += OnUserDeath;
    }
    private void UnSubscribeToEvents()
    {
        AIUnit.OnSpawn -= OnUserSpawn;
        AIUnit.OnDeath -= OnUserDeath;

        Structure.OnSpawn -= OnUserSpawn;
        Structure.OnDeath -= OnUserDeath;
    }

    private void OnUserSpawn(IMWRUser user)
    {
        Debug.Log($"call:{user}");
        if (CurrentMWR.TryAssignUser(user))
        {

        }
        else
        {
            Debug.Log($"initial positioning failed for: {user}");
        }
        
    }
    private void OnUserDeath(IMWRUser user)
    {
        Vector2Int cords = user.CurrentCoordintates;
        CurrentMWR.worldRepresentation[cords.x, cords.y] = true;        
    }

    private void CreateMWR()
    {
        CurrentMWR = new MovementWorldRepresentation((GridGraph)pathfinder.data.FindGraphOfType(typeof(GridGraph)));
    }

    private void SetUpVariablesInNeeded()
    {
        if (pathfinder == null)
        {
            pathfinder = GetComponent<AstarPath>();
            if (pathfinder == null)
            {
                pathfinder = FindObjectOfType<AstarPath>();
                Debug.Assert(pathfinder != null, "must put a AStarPath!");
            }
        }
    }
}
