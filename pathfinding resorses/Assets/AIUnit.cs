using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Pathfinding;

public class AIUnit : MonoBehaviour
{
    public List<AIAgent> childs = new List<AIAgent>();

    public AIUnitData data;

    public IAstarAI movementAI;

    
    public Vector2 GetCohesionPosition(AIAgent requester)
    {
        //esto debe luego entregar posiciones tomando en cuenta la formacion 
        return (Vector2)transform.position;
    }

    public void Start()
    {
        movementAI = GetComponent<IAstarAI>();
    }


    //public AIAgent[] Childs { get; }
    //public Vector2[] FormationOffset { get; }



    //public bool OnDestination { get; }

    //public 
    //public Vector2? GetChildDesiredPosition(AIAgent requester)
    //{
    //    int index = Array.IndexOf(Childs, requester);
    //    if (index == -1)
    //        return null;

    //    if (OnDestination)
    //        return (Vector2)transform.position;
    //    else
    //    {
    //        Vector2 desPos = (Vector2)transform.position + FormationOffset[index];
    //        return desPos;
    //    }
    //}
}
