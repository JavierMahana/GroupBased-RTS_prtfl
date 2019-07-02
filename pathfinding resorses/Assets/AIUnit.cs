using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Pathfinding;


public class AIUnit : MonoBehaviour
{
    public bool gizmos;
    private bool started = false;

    public FormationData formationData;
    public List<AIAgent> children = new List<AIAgent>();
    private AIAgent[] orderedChildren;
    private Vector2[] formationSlots;

    public AIUnitData data;

    public IAstarAI movementAI;
    public bool Moving
    {
        get { return movementAI.velocity.sqrMagnitude > float.Epsilon; }
    }
    
    public Vector2 GetCohesionPosition(AIAgent requester)
    {
        if (Moving)
        {
            return (Vector2)transform.position + (Vector2)movementAI.velocity.normalized * 0.9f;
        }
        int index = Array.IndexOf(orderedChildren, requester);
        return (Vector2)transform.position + formationSlots[index] - new Vector2(0.5f, 0.5f);
    }

    public void Start()
    {
        started = true;  
        movementAI = GetComponent<IAstarAI>();
        UpdateFormationAndChildren();
    }

    public void UpdateFormationAndChildren()
    {
        orderedChildren = new AIAgent[children.Count];
        List<AIAgent> childrenCopy = new List<AIAgent>(children);

        int index = 0;
        while (childrenCopy.Count > 0)
        {
            AIAgent current = childrenCopy[0];
            float minYValue = current.transform.position.y;
            for (int i = 1; i < childrenCopy.Count; i++)
            {
                AIAgent temp = childrenCopy[i];
                float yTemp = temp.transform.position.y;

                if (minYValue > yTemp)
                {
                    current = temp;
                    minYValue = yTemp;
                }
            }
            orderedChildren[index] = current;
            childrenCopy.Remove(current);
            index++;
        }
        Debug.Log("actualizando slots");
        Vector2Int key = new Vector2Int(orderedChildren.Length, (int)data.type);
        if (! formationData.FormationOffsetData.TryGetValue(key, out formationSlots))
        {
            Debug.LogError("invalid pair");
        } 

    }
    private void OnDrawGizmos()
    {
        if (gizmos)
        {
            Gizmos.color = Color.blue;
            if (started)
            {
                Gizmos.DrawWireSphere((Vector2)transform.position + (Vector2)movementAI.velocity.normalized * 0.9f, data.closeUpBehaviourArea);
            }
            else
                Gizmos.DrawWireSphere((Vector2)transform.position, data.closeUpBehaviourArea);

            Gizmos.color = Color.black;
            if (started)
            {
                Gizmos.DrawWireSphere((Vector2)transform.position + (Vector2)movementAI.velocity.normalized * 0.9f, data.closeUpBehaviourMaxIntensityArea);
            }
            else
                Gizmos.DrawWireSphere((Vector2)transform.position, data.closeUpBehaviourMaxIntensityArea);

        }
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
