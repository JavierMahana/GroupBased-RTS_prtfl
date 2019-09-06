using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Pathfinding;
using Lean.Pool;
using UnityEngine.EventSystems;

//podira luego transformarlo en una cascara, en donde hay muchos sistemas, pero la gracia es que acá es donde se asignan los valores
public class AIUnit : SerializedMonoBehaviour, IMWRUser, ISelectable, IKillable
{
    #region displayable
    //displayable shit
    public string DisplayName => name;
    public Sprite DisplayIcon => data.displayebleData.icon;
    public string DisplayDesc => data.displayebleData.desc;
    public Health DisplayHealth => null;
    public Vector2Int? CapacityDisplay => new Vector2Int(children.Count, (int)data.maxSize);
    public List<IDisplayable> DependentDisplayables => children.ConvertAll(x => (IDisplayable)x);
    public List<AbilityData> Abilities { get; }
    public ComandCardButton[,] CommandCardButtons { get; }
    public Dictionary<BaseButtonData, BaseCommand> ButtonToCommandDictionary => buttonToCommandDictionary;
    private Dictionary<BaseButtonData, BaseCommand> buttonToCommandDictionary = new Dictionary<BaseButtonData, BaseCommand>();

    
    #endregion


    //used by the managers to listen to this
    public static event Action<AIUnit> OnSpawn = delegate { };       
    public event KillableEvent OnDeath = delegate { };
    public event SelectionEvent SelectionStateChanged = delegate { };

    public bool selected = false;
    public bool gizmos;
    public Team team;

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
    public virtual AIUnitData Data => data;
    [SerializeField]
    [Required]
    private AIUnitData data = null;

    private void UpdatePosibleTargets()
    {
        StopHearingFromOldPossibleTargets();

        if (Moving)
        {
            posibleTargets = null;
        }

        Vector2 position = transform.position;

        List<ITargetable> filteredTargets = GetFilteredTargetableInRange(position);
        if (filteredTargets == null || filteredTargets.Count == 0)
        {
            posibleTargets = null;            
        }
        else
        {
            List<ITargetable> targets = GetPrioritizedTargets(filteredTargets, position);
            posibleTargets = targets;            
        }


        ListenPosibleTargetsEvents();
    }

    private void OnPosibleTargetDeath(IKillable posibleTarget)
    {
        
        UpdatePosibleTargets();
    }
    private void StopHearingFromOldPossibleTargets()
    {
        if (posibleTargets == null || posibleTargets.Count == 0) return;
        foreach (ITargetable target in posibleTargets)
        {
            target.OnDeath -= OnPosibleTargetDeath;
        }
    }
    private void ListenPosibleTargetsEvents()
    {
        if (posibleTargets == null || posibleTargets.Count == 0) return;

        foreach (IKillable target in posibleTargets)
        {
            target.OnDeath += OnPosibleTargetDeath;
        }
    }
    //[HideInInspector]
    public List<ITargetable> posibleTargets = null;


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
    public IKillable KillableEntity { get { return this; } }
    public GameObject GameObject { get { return gameObject; } }
    //updated by MWRManager at "OnSpawn"
    public Vector2Int CurrentCoordintates { get; set; }
    private Vector2Int currentCoordinates = new Vector2Int(int.MaxValue, int.MaxValue);
    public MovementWorldRepresentation MWR { get; set; }

    public Team Team { get { return team; } }

    public virtual UIMode UIMode { get { return UIMode.UNIT; } }


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
    
    //ojo con esto y el awake. Ya que hacen muchas cosas similares.
    public void AddNewChild(AIAgent child)
    {
        if (children.Count >= (int)Data.maxSize) Debug.LogError("chindren count is full. You can't add more");
        children.Add(child);

        child.OnDeath += OnChildrenDeath;
        child.parent = this;

        SelectionStateChanged();
        UpdateFormationAndChildren();
    }    
    public void OnChildrenDeath(IKillable child)
    {
        if (!children.Remove((AIAgent)child)) Debug.LogError("the agent isn't your children");
        child.OnDeath -= OnChildrenDeath;

        if (children.Count == 0)
        {
            LeanPool.Despawn(this);            
            return;
        }

        SelectionStateChanged();
        UpdateFormationAndChildren();
    }


    private List<ITargetable> GetFilteredTargetableInRange(Vector2 position)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(position, Data.detectionRadious, Data.detectableLayers);
        List<ITargetable> allEntities = GetAllTargetable(allColliders);
        List<ITargetable> filteredTargetable = Data.targetFilter.FilterTargets(this, allEntities);
        return filteredTargetable;
    }
    private List<ITargetable> GetPrioritizedTargets(List<ITargetable> filteredPosibleTargets, Vector2 position)
    {
        bool haveAgent = false;

        ITargetable closestPosibleTarget = null;//estructuras?
        float closestStructureSqrDist = float.MaxValue;
        AIAgent closestAgent = null;
        float closestAgentSqrDist = float.MaxValue;
        foreach (ITargetable targetable in filteredPosibleTargets)
        {
            Vector2 targetPos = targetable.GameObject.transform.position;
            if (targetable is AIAgent)
            {
                haveAgent = true;
                float sqrDist = Vector2Utilities.SqrDistance(targetPos, position);
                if (sqrDist < closestAgentSqrDist)
                {
                    closestAgent = (AIAgent)targetable;
                    closestAgentSqrDist = sqrDist;
                }
            }
            else
            {
                if (haveAgent) continue;

                float sqrDist = Vector2Utilities.SqrDistance(targetPos, position);
                if (sqrDist < closestStructureSqrDist)
                {
                    closestPosibleTarget = targetable;
                    closestStructureSqrDist = sqrDist;
                }
            }
        }

        List<ITargetable> returnTargets = new List<ITargetable>();
        if (haveAgent)
        {
            returnTargets = closestAgent.parent.children.ConvertAll(x => (ITargetable)x);
            return returnTargets;
        }
        else if (closestPosibleTarget != null)
        {
            returnTargets.Add(closestPosibleTarget);
            return returnTargets;
        }
        else
            return null;
    }
    private List<ITargetable> GetAllTargetable(Collider2D[] posibleTargets)
    {
        List<ITargetable> targetables = new List<ITargetable>();
        for (int i = 0; i < posibleTargets.Length; i++)
        {
            Collider2D curr = posibleTargets[i];
            ITargetable currTarget = curr.GetComponent<ITargetable>();
            if (currTarget == null) continue;
            else targetables.Add(currTarget);
        }
        return targetables;
    }

    private bool safeEventInvoke = false;
    private void Awake()
    {
        safeEventInvoke = false;
        foreach (AIAgent child in children)
        {
            child.OnDeath += OnChildrenDeath;
            child.parent = this;            
        }
        movementAI = GetComponent<IAstarAI>();
        SelectionStateChanged();

        if (children.Count != 0)
        {
            UpdateFormationAndChildren();
        }
        
    }
    private void OnApplicationQuit()
    {
        quit = true;
        foreach (IKillable child in children)
        {
            child.OnDeath -= OnChildrenDeath;
        }
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
        if(!quit)
            OnDeath(this);
    }
    private bool quit;
    

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
            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraController.CAMERA_TO_WORLD_DISTANCE));
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
        Vector2Int key = new Vector2Int(orderedChildren.Length, (int)Data.maxSize);
        if (! Data.formationData.FormationOffsetData.TryGetValue(key, out formationSlots))
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

    public void Select(SelectionManager manager)
    {
        selected = true;
        Debug.Log($"selected {this}");
    }

    public void Deselect(SelectionManager manager)
    {
        selected = false;
        Debug.Log($"deselect {this}");
    }

    public ISelectable GetSelectable()
    {
        return this;
    }
}
