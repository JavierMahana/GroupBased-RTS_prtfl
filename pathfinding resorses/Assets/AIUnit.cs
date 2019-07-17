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

    [HideInInspector]
    public List<IEntity> PosibleTargets
    {
        get
        {
            return posibleTargets;
        }
        private set
        {
            posibleTargets = value;
        }
    }
    private List<IEntity> posibleTargets = null;

    [HideInInspector]
    public Team Team {
        get
        {
            if (unitTeam != null)
                return unitTeam;
            else
            {
                unitTeam = children[0].Team;
                return unitTeam;
            }
                
        }
    }
    private Team unitTeam;

    #region macro behaviour
    public IMacroBehaviour independentMacro;
    public IMacroBehaviour dependentMacro;
    [HideInInspector]
    public IMacroBehaviour ActiveMacro {
        get
        {
            if (mbTimer >= data.MB_UPDATE_TIME || activeMacro == null)
            {
                IMacroBehaviour temp = UpdateMacroBehaviour();
                if(temp != activeMacro) 
                activeMacro = temp;
                mbTimer = 0;
            }
                

            return activeMacro;
        }
    }
    private IMacroBehaviour activeMacro;
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
    public IMacroBehaviour UpdateMacroBehaviour()
    {
        
        if (Moving)
        {
            PosibleTargets = null;
            return independentMacro;
        }
        
        Vector2 position = transform.position;

        List<IEntity> filteredEntities = GetFilteredEntitiesInRange(position);
        if (filteredEntities == null || filteredEntities.Count == 0)
        {
            PosibleTargets = null;
            return independentMacro;
        }


        List<IEntity> targets = GetPrioritizedTargets(filteredEntities, position);
        PosibleTargets = targets;
        return dependentMacro;
    }

    private List<IEntity> GetFilteredEntitiesInRange(Vector2 position)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(position, data.detectionRadious, data.detectableLayers);
        List<IEntity> allEntities = GetAllEntities(allColliders);
        List<IEntity> filteredEntities = data.targetFilter.FilterEntities(this, allEntities);
        return filteredEntities;
    }
    private List<IEntity> GetPrioritizedTargets(List<IEntity> filteredPosibleTargets, Vector2 position)
    {
        bool haveAgent = false;

        IEntity closestEntity = null;//eventualmente esto será una estructura
        float closestEntitySqrDist = float.MaxValue;
        AIAgent closestAgent = null;
        float closestAgentSqrDist = float.MaxValue;
        foreach (IEntity entity in filteredPosibleTargets)
        {
            Vector2 entityPos = entity.GameObject.transform.position;
            if (entity is AIAgent)
            {
                haveAgent = true;
                float sqrDist = Vector2Utilities.SqrDistance(entityPos, position);
                if (sqrDist < closestAgentSqrDist)
                {
                    closestAgent = (AIAgent)entity;
                    closestAgentSqrDist = sqrDist;
                }
            }
            else
            {
                if (haveAgent) continue;

                float sqrDist = Vector2Utilities.SqrDistance(entityPos, position);
                if (sqrDist < closestEntitySqrDist)
                {
                    closestEntity = entity;
                    closestEntitySqrDist = sqrDist;
                }
            }
        }

        List<IEntity> returnTargets = new List<IEntity>();
        if (haveAgent)
        {
            returnTargets = closestAgent.parent.children.ConvertAll(x => (IEntity)x);
            return returnTargets;
        }
        else if (closestEntity != null)
        {
            returnTargets.Add(closestEntity);
            return returnTargets;
        }
        else
            return null;
    }
    private List<IEntity> GetAllEntities(Collider2D[] posibleEntities)
    {
        List<IEntity> entities = new List<IEntity>();
        for (int i = 0; i < posibleEntities.Length; i++)
        {
            Collider2D curr = posibleEntities[i];
            IEntity currEntity = curr.GetComponent<IEntity>();
            if (currEntity == null) continue;
            else entities.Add(currEntity);
        }
        return entities;
    }

    public void Start()
    {
        started = true;  
        movementAI = GetComponent<IAstarAI>();
        UpdateFormationAndChildren();
    }
    private void Update()
    {
        mbTimer += Time.deltaTime;
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
    //    if (gizmos)
    //    {
    //        Gizmos.color = Color.blue;
    //        if (started)
    //        {
    //            Gizmos.DrawWireSphere((Vector2)transform.position + (Vector2)movementAI.velocity.normalized * 0.9f, data.closeUpBehaviourArea);
    //        }
    //        else
    //            Gizmos.DrawWireSphere((Vector2)transform.position, data.closeUpBehaviourArea);

    //        Gizmos.color = Color.black;
    //        if (started)
    //        {
    //            Gizmos.DrawWireSphere((Vector2)transform.position + (Vector2)movementAI.velocity.normalized * 0.9f, data.closeUpBehaviourMaxIntensityArea);
    //        }
    //        else
    //            Gizmos.DrawWireSphere((Vector2)transform.position, data.closeUpBehaviourMaxIntensityArea);

    //    }
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
