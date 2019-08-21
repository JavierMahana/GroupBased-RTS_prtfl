using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;
using System;
using Lean.Pool;

[RequireComponent(typeof(Health))]
public class AIAgent : Entity, ITriggerSelection
{
    //hacer un parametro de posicion, el cual adquiera la posicion solo una vez por frame.

    #region Variables set in the inspector
    public AIAgentData data;
    public bool debug;
    public bool gizmos;    
    public AIUnit parent;
    public EntityAction entityAction;
    #endregion

    #region Variables set in initialization
    private Health health;
    public Rigidbody2D body;
    #endregion

    [HideInInspector]
    public float repathTimer;
    private float bsChangeTimer = 0;
    private float bsTimer = 0;
    public float timeToRepath = 1;


    private bool shouldRecalculateNow = false;
    private bool dontRecalculate = false;
    private bool acting = false;
    private bool currentlyUsingSB = false;
    private bool sbConsistency;
    private Vector2 currentStuckDestination;
    private float movementDelta = float.MaxValue;
    private float movementDeltaTimer = 0;
    private Vector2 comparationPosition = Vector2.positiveInfinity; //set and used on "MovementDelta"

    private Vector2 prevPosition1, prevPosition2;
    private float lastDelta;
    private int prevFrame;


    private bool BSJustChanged
    {
        get
        {
            return bsChangeTimer < data.timeBSJustChangedIsTrue;
        }
    }        
    private bool InActionRangeToDestination
    {
        get
        {
            float distanceToDestination = Vector2.Distance(transform.position, Destination);
            if (distanceToDestination < (data.reachDestinationMargin + entityAction.BaseData.rangeOfAction))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private bool Moving
    {
        get
        {
            if (GetMovementDeltaAndUpdateIfNeeded() < data.lowerMovementDeltaIsStoped)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
    private bool OnActState
    {
        get { return data.actionBehaviour == ActiveBehaviourSet; }
    }
    public Entity Target
    {
        get
        {
            List<Entity> options = parent.posibleTargets;
            if (options == null || options.Count == 0) return null;

            Vector2 position = transform.position;
            Entity returnEntity = options[0];
            float closestEntitySqrDist = Vector2Utilities.SqrDistance(position, returnEntity.GameObject.transform.position);
            for (int i = 1; i < options.Count; i++)
            {
                Entity currEntity = options[i];
                float currSqrDist = Vector2Utilities.SqrDistance(position, currEntity.GameObject.transform.position);
                if (currSqrDist < closestEntitySqrDist)
                {
                    returnEntity = currEntity;
                    closestEntitySqrDist = currSqrDist;
                }
            }
            return returnEntity;
        }
    }
    public IAstarAI AI { get; private set; }
    public override Health Health { get { return health; } }
    public override float Radious
    {
        get
        { return data.radious; }
    }
    public override Shape BodyShape
    {
        get
        { return data.shape; }
    }
    public override GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }
    public Vector2 Destination
    {
        get { return parent.ActiveMacro.GetDesiredDestination(this); }
    }
    //public Vector2 GeneralVelocity - Velocidad de la que toma referencia las animaciónes
    public Vector2 Velocity
    {
        get
        {
            return lastDelta > 0.000001f ? (prevPosition1 - prevPosition2) / lastDelta : Vector2.zero; ;
        }
    }    
    public override Team Team { get { return parent.team; } }

    public IBehaviourSet ActiveBehaviourSet
    {
        get
        {
            if (!dontRecalculate && (bsTimer >= data.BS_UPDATE_TIME || activeBehaviourSet == null || shouldRecalculateNow))
            {
                activeBehaviourSet = parent.ActiveMacro.GetBehaviourSet(this);
                bsTimer = 0;
                shouldRecalculateNow = false;
            }
            return activeBehaviourSet;
        }
    }
    private IBehaviourSet activeBehaviourSet;


    public ISelectable GetSelectable()
    {
        return parent;
    }
    private void OnActionFinished()
    {
        acting = false;
        shouldRecalculateNow = true;
        dontRecalculate = false;
    }
    private void SendActionStartEvents()
    {
        acting = true;
        dontRecalculate = true;
        ActionBegan(Target);
    }
    private void ResetMovementDelta()
    {
        movementDelta = float.MaxValue;
        movementDeltaTimer = 0;
        comparationPosition = transform.position;
    }
    private float GetMovementDeltaAndUpdateIfNeeded()
    {
        if (movementDeltaTimer >= data.movementDeltaUpdateTime)
        {
            movementDelta = Vector2.Distance(comparationPosition, transform.position);
            movementDeltaTimer = 0;
            comparationPosition = transform.position;
        }

        return movementDelta;

    }
    public void OnMBUpdate()
    {
        bsChangeTimer = 0;
    }
    
    public bool UseStuckBehaviour(out Vector2? stuckDestination)
    {
        if (currentlyUsingSB)
        {
            if (!Moving)
            {
                stuckDestination = currentStuckDestination = CalculateStuckDestination(parent.data.obstacleLayerMask);
            }


            float sqrDistToStuckDest = Vector2Utilities.SqrDistance((Vector2)transform.position, currentStuckDestination);
            if (sqrDistToStuckDest < Mathf.Pow(data.reachDestinationMargin, 2) || InActionRangeToDestination)
            {
                stuckDestination = null;
                currentlyUsingSB = false;
                return false;
            }
            else
            {
                stuckDestination = currentStuckDestination;
                return true;
            }
            //debo revisar si es que se vuelva a atrapar luego de ya estar atrapado. 
        }
        else
        {
            if (!Moving && !InActionRangeToDestination && !BSJustChanged)
            {
                stuckDestination = currentStuckDestination = CalculateStuckDestination(parent.data.obstacleLayerMask);
                currentlyUsingSB = true;
                ResetMovementDelta();
                return true;
            }
            else
            {
                stuckDestination = null;
                currentlyUsingSB = false;
                return false;
            }
        }
    }
    private void StartUpVariables()
    {
        ResetMovementDelta();
        
        sbConsistency = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
    } 
    private Vector2 CalculateStuckDestination(int obstacleLayerMask)
    {
        Vector2 position = transform.position;
        Vector2 dir = (Destination - position).normalized;

        ////int rndm = Random.Range(0, 2);
        //Vector2 perpendicular = rndm == 0 ? Vector2.Perpendicular(dir) : -Vector2.Perpendicular(dir);
        Vector2 perpendicular = sbConsistency ? Vector2.Perpendicular(dir) : -Vector2.Perpendicular(dir);

        Vector2 tempStuckDest = position + perpendicular * data.stuckDesinationOffset;
        RaycastHit2D hit = Physics2D.Linecast(position, tempStuckDest, obstacleLayerMask); //este linecast no considera el area del agente.
        if (hit.collider != null)
        {
            sbConsistency = !sbConsistency;

            tempStuckDest = position - perpendicular * data.stuckDesinationOffset;
            #region checking for indesirable cases            
            hit = Physics2D.Linecast(position, tempStuckDest, obstacleLayerMask);
            if (hit.collider != null)
            {
                Debug.LogWarning("modifique el valor de # data.stuckDesinationOffset # ya que su valor parece ser muy alto. Stuck destination: Destination");
                return Destination;
            }
            #endregion
            else return tempStuckDest;
        }
        else return tempStuckDest;
    }


    private void Awake()
    {
        //started = true;
        entityAction = GetComponent<EntityAction>();
        if (entityAction == null) Debug.LogError("Entities must have an action");

        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        GetAndSetAI();
        StartUpVariables();

        prevPosition1 = prevPosition2 = transform.position;


    }
    private void OnEnable()
    {
        health.InvokeDeath += OnDeath;
        entityAction.ActionEnded += OnActionFinished;
    }
    private void OnDisable()
    {
        health.InvokeDeath -= OnDeath;
        entityAction.ActionEnded -= OnActionFinished;
    }

    private void Update()
    {
        if (body == null)
        {
            float delta = Time.deltaTime;
            transform.position = ActiveBehaviourSet.CalculateDesiredPosition(this, delta);

            UpdateTimers(delta);
            UpdateVelocityParams(delta);
        }

    }
    private void FixedUpdate()
    {
        if (body != null)
        {
            float delta = Time.fixedDeltaTime;
            body.MovePosition(ActiveBehaviourSet.CalculateDesiredPosition(this, delta));
            
            UpdateTimers(delta);
            UpdateVelocityParams(delta);


            //if (debug) Debug.Log($"Moving: {Moving.ToString()}|Acting: {Acting.ToString()} |In SB: {currentlyUsingSB}|MB just changed: {BSJustChanged}");
        }

        if (OnActState && !acting)
        {
            SendActionStartEvents();
        }

    }


    private void UpdateVelocityParams(float deltaTime)
    {
        lastDelta = deltaTime;

        int currentFrame = Time.frameCount;

        if (currentFrame != prevFrame) prevPosition2 = prevPosition1;
        prevPosition1 = transform.position;
        prevFrame = currentFrame;
    }
    private void UpdateTimers(float deltaTime)
    {
        bsChangeTimer += deltaTime;
        bsTimer += deltaTime;
        repathTimer += deltaTime;
        movementDeltaTimer += deltaTime;
    }
    
    private void GetAndSetAI()
    {
        AI = GetComponent<IAstarAI>();
        AI.canMove = false;
        AI.canSearch = false;
        AI.radius = data.radious;
        AI.maxSpeed = data.maxSpeed;


        if (AI is AIPath)
        {
            //nose como hacerlo la verdad... uwu
        }
        //hacer que los parametros dentro de la IA coincidan con los que se especifican acá.
    }
    private void OnDrawGizmos()
    {
        //if (started)
        //{

        //    if (Acting)
        //    {
        //        Gizmos.color = Color.cyan;
        //        Gizmos.DrawSphere(transform.position, data.radious);
        //    }
        //    else if (Stuck)
        //    {

        //        Gizmos.color = Color.magenta;
        //        Gizmos.DrawSphere(transform.position, data.radious);
        //    }
        //}
        if (gizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, data.radious * data.separationRangeInRadious);
        }
    }


    private void OnDeath()
    {
        OnEntityDeath(this);
        LeanPool.Despawn(this);
    }
}
