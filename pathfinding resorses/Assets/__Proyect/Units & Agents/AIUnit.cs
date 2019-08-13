using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Pathfinding;
using Lean.Pool;

//quizas las unidades podrian heredar de una interfaz que controle el ocupar espacios en el grafico de movimiento.
public class AIUnit : SerializedMonoBehaviour, IMWRUser
{
    //ahora mismo el sistema no es muy bueno, pero alv.
    private const float CAMERA_TO_WORLD_DISTANCE = 10f;

    public bool selected = false;
    public bool gizmos;
    public Team team;

    

    public static event Action<AIUnit> OnDeath = delegate { };
    public static event Action<AIUnit> OnSpawn = delegate { };

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
    private void UpdatePosibleTargets()
    {
        StopHearingFromOldPossibleTargets();

        if (Moving)
        {
            posibleTargets = null;
        }

        Vector2 position = transform.position;

        List<Entity> filteredEntities = GetFilteredEntitiesInRange(position);
        if (filteredEntities == null || filteredEntities.Count == 0)
        {
            posibleTargets = null;            
        }
        else
        {
            List<Entity> targets = GetPrioritizedTargets(filteredEntities, position);
            posibleTargets = targets;            
        }


        ListenPosibleTargetsEvents();
    }

    private void OnPosibleTargetDeath(Entity entity)
    {
        UpdatePosibleTargets();
    }
    private void StopHearingFromOldPossibleTargets()
    {
        if (posibleTargets == null || posibleTargets.Count == 0) return;
        foreach (Entity entity in posibleTargets)
        {
            entity.OnEntityDeath -= OnPosibleTargetDeath;
        }
    }
    private void ListenPosibleTargetsEvents()
    {
        if (posibleTargets == null || posibleTargets.Count == 0) return;

        foreach (Entity entity in posibleTargets)
        {
            entity.OnEntityDeath += OnPosibleTargetDeath;
        }
    }
    [HideInInspector]
    public List<Entity> posibleTargets = null;


    #region macro behaviour
    public IMacroBehaviour independentMacro;
    public IMacroBehaviour dependentMacro;
    [HideInInspector]
    public IMacroBehaviour ActiveMacro {
        get
        {
            if (mbTimer >= AIUnitData.MB_UPDATE_TIME || activeMacro == null)
            {
                IMacroBehaviour temp = UpdateMacroBehaviour();
                if (temp != activeMacro) OnMBUpdate();

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

    public GameObject GameObject { get { return gameObject; } }
    //updated by MWRManager at "OnSpawn"
    public Vector2Int CurrentCoordintates { get; set; }
    private Vector2Int currentCoordinates = new Vector2Int(int.MaxValue, int.MaxValue);
    public MovementWorldRepresentation MWR { get; set; }



    private void OnMBUpdate()
    {
        //update children timers
        foreach (AIAgent child in children)
        {
            child.OnMBUpdate();
        }
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

        UpdatePosibleTargets();
        if (posibleTargets != null)
        {
            return dependentMacro;
        }
        else
        {
            return independentMacro;
        }
    }
    
    public void AddNewChild(AIAgent agent)
    {
        if (children.Count >= (int)data.maxSize) Debug.LogError("chindren count is full. You can't add more");

        children.Add(agent);
        agent.OnEntityDeath += OnChildrenDeath;
        agent.parent = this;

        UpdateFormationAndChildren();
    }
    //acá se llama la muerte de una unidad
    public void OnChildrenDeath(Entity child)
    {
        if (!children.Remove((AIAgent)child)) Debug.LogError("the agent isn't your children");
        child.OnEntityDeath -= OnChildrenDeath;

        if (children.Count == 0)
        {
            //OnDeath(this);
            //acá se llama a el sistema de despawneo
            LeanPool.Despawn(this);
            //gameObject.SetActive(false);
            return;
        }

        UpdateFormationAndChildren();
    }


    private List<Entity> GetFilteredEntitiesInRange(Vector2 position)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(position, data.detectionRadious, data.detectableLayers);
        List<Entity> allEntities = GetAllEntities(allColliders);
        List<Entity> filteredEntities = data.targetFilter.FilterEntities(this, allEntities);
        return filteredEntities;
    }
    private List<Entity> GetPrioritizedTargets(List<Entity> filteredPosibleTargets, Vector2 position)
    {
        bool haveAgent = false;

        Entity closestEntity = null;//eventualmente esto será una estructura
        float closestEntitySqrDist = float.MaxValue;
        AIAgent closestAgent = null;
        float closestAgentSqrDist = float.MaxValue;
        foreach (Entity entity in filteredPosibleTargets)
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

        List<Entity> returnTargets = new List<Entity>();
        if (haveAgent)
        {
            returnTargets = closestAgent.parent.children.ConvertAll(x => (Entity)x);
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
    private List<Entity> GetAllEntities(Collider2D[] posibleEntities)
    {
        List<Entity> entities = new List<Entity>();
        for (int i = 0; i < posibleEntities.Length; i++)
        {
            Collider2D curr = posibleEntities[i];
            Entity currEntity = curr.GetComponent<Entity>();
            if (currEntity == null) continue;
            else entities.Add(currEntity);
        }
        return entities;
    }

    private bool safeEventInvoke = false;
    private void Awake()
    {
        safeEventInvoke = false;
        foreach (AIAgent child in children)
        {
            child.OnEntityDeath += OnChildrenDeath;
            child.parent = this;            
        }
        movementAI = GetComponent<IAstarAI>();
        UpdateFormationAndChildren();

    }




    private void LateStart()
    {
        safeEventInvoke = true;
        OnSpawn(this);

    }
    private void OnEnable()
    {
        if (safeEventInvoke)
        {
            OnSpawn(this);
        }
            
    }
    private void OnDisable()
    {
        if (safeEventInvoke)
        {
            OnDeath(this);
        }
            
    }

    private void Update()
    {
        //run only in first update
        if (!safeEventInvoke)
        {
            LateStart();
        }

        mbTimer += Time.deltaTime;
        if (selected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3( Input.mousePosition.x, Input.mousePosition.y, CAMERA_TO_WORLD_DISTANCE));
                Vector2Int cell = MWR.GetCell(point);                

                if (MWR.MoveToCell(this, cell))
                {

                }
                else
                {
                    Debug.Log("movimiento fallido");
                }
                
            }            
        }

        if (MWR != null)
        {
            movementAI.destination = MWR.GetCellPosition(CurrentCoordintates);
        }
         
        //Debug.Log($"unit velocity = {movementAI.velocity.magnitude} | {movementAI.velocity}");
    }
    private void UpdateFormationAndChildren()
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
        Vector2Int key = new Vector2Int(orderedChildren.Length, (int)data.maxSize);
        if (! data.formationData.FormationOffsetData.TryGetValue(key, out formationSlots))
        {
            Debug.LogError($"invalid pair; key:{key}|at: {this}");
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
    
}
