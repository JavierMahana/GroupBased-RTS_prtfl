using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Pathfinding;


public class AIUnit : SerializedMonoBehaviour
{
    public bool gizmos;
    private bool started = false;

    public FormationData formationData;
    public List<AIAgent> children = new List<AIAgent>();
    private AIAgent[] orderedChildren;
    private Vector2[] formationSlots;

    //cuando se aruegue el timer
    //el buscar la unidad más sercana de una unidad, esta busqueda genera una actualización en las dos unidades.
    //eso se hace mediante el paramtro set. Ahí se reiniciaria el contador tambien.
    public AIUnit ClosestUnit
    {
        get
        {
            AIUnit[] allUnits = FindObjectsOfType<AIUnit>();
            Vector2 pos = transform.position;
            AIUnit closestUnit = null;
            float closestsqrDist = float.MaxValue;
            for (int i = 0; i < allUnits.Length; i++)
            {
                AIUnit current = allUnits[i];
                if (current == this)
                    continue;
                float currentSqrDist = Vector2Utilities.SqrDistance(pos, (Vector2)current.transform.position);
                if (currentSqrDist < closestsqrDist)
                {
                    closestsqrDist = currentSqrDist;
                    closestUnit = current;
                }
            }
            return closestUnit;
        }
    }

    public AIUnitData data;

    #region macro behaviour
    public IMacroBehaviour temporalMacro;
    [HideInInspector]
    public IMacroBehaviour ActiveMacro {
        get
        {
            if (mbTimer >= MB_UPDATE_TIME || activeMacro == null)
            {
                activeMacro = GetUpdatedMacroBehaviour();
                mbTimer = 0;
            }
                

            return activeMacro;
        }
    }
    private IMacroBehaviour activeMacro;
    public float MB_UPDATE_TIME = 5F;//hacer private constante luego de probar un tiempo bueno
    private float mbTimer = 0;
    #endregion

    [HideInInspector]
    public IAstarAI movementAI;
    public bool Moving
    {
        get { return movementAI.velocity.sqrMagnitude > 0.0001; }
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
    public IMacroBehaviour GetUpdatedMacroBehaviour()
    {
        return temporalMacro;
    }

    public void Start()
    {
        started = true;  
        movementAI = GetComponent<IAstarAI>();
        UpdateFormationAndChildren();
    }
    private void Update
        ()
    {
        //Debug.Log($"unit velocity = {movementAI.velocity.magnitude} | {movementAI.velocity}");
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
