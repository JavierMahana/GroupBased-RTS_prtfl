using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class AIAgent : SerializedMonoBehaviour, IEntity
{
    public bool debug;
    public bool gizmos;
    //private bool started = false;
    public bool test;
    //hacer un parametro de posicion, el cual adquiera la posicion solo una vez por frame.

    public AIUnit parent;


    
    public bool Stuck
    {
        get
        {
            if (stuck)
            {
                float distToStuckDest = Vector2Utilities.SqrDistance(transform.position, stuckDestination);
                if (distToStuckDest <= data.stuckSqrDistanceMargin)
                    stuck = false;
            }

            else
            {
                if (MovementDelta < data.stuckMovmentDeltaTreshold)
                {
                    stuck = true;
                    SetStuckDestination();
                }
            }

            return stuck;
        }
    }
    private bool stuck;
    private float MovementDelta
    {
        get
        {
            if (movementDeltaTimer >= data.movementDeltaUpdateTime)
            {
                movementDelta = Vector2Utilities.SqrDistance(comparationPosition, transform.position);
                movementDeltaTimer = 0;
                comparationPosition = transform.position;
            }

            return movementDelta;
        }
    }
    private float movementDelta = float.MaxValue;
    private float movementDeltaTimer = 0;
    private Vector2 comparationPosition =  Vector2.positiveInfinity; //set on "MovementDelta"
    public Vector2 stuckDestination = Vector2.positiveInfinity;

    public bool Acting
    {
        get { return data.actionBehaviour == ActiveBehaviourSet; }
    }
    public IEntity Target
    {
        get
        {
            List<IEntity> options = parent.PosibleTargets;
            if (options == null) return null;

            Vector2 position = transform.position;
            IEntity returnEntity = options[0];
            float closestEntitySqrDist = Vector2Utilities.SqrDistance(position, returnEntity.GameObject.transform.position);
            for (int i = 1; i < options.Count; i++)
            {
                IEntity currEntity = options[i];
                float currSqrDist = Vector2Utilities.SqrDistance(position, currEntity.GameObject.transform.position);
                if(currSqrDist < closestEntitySqrDist)
                {
                    returnEntity = currEntity;
                    closestEntitySqrDist = currSqrDist;
                }
            }
            return returnEntity;
        }
    }
    public IAstarAI AI { get; private set; }
    public float timeToRepath = 1;
    [HideInInspector]
    public float repathTimer;

    public float Radious
    {
        get
        { return data.radious; }
    }
    public Shape BodyShape
    {
        get
        { return data.shape; }
    }
    public GameObject GameObject
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
            return lastDelta > 0.000001f  ? (prevPosition1 - prevPosition2) / lastDelta : Vector2.zero; ;
        }
    }
    private Vector2 prevPosition1, prevPosition2;
    private float lastDelta;
    private int prevFrame;

    //agruegar timer y implementar la fncionalidad al bh de avoidance. :v
    //public AIAgent ObstacleToEvade
    //{
    //    get
    //    {
    //        SteeringBehaviour.GetClosestAgent(this, transform.position, parent.ClosestUnit.children)
    //    }
    //}
    public int ciclesToRecalculateObstacle = 8;
    private int obstacleCicles;

    public AIAgentData data;
    

    public IBehaviourSet ActiveBehaviourSet
    {
        get
        {
            if (bsTimer >= data.BS_UPDATE_TIME || activeBehaviourSet == null)
            {
                activeBehaviourSet = parent.ActiveMacro.GetBehaviourSet(this);
                bsTimer = 0;
            }
                

            return activeBehaviourSet;
        }
    }
    private IBehaviourSet activeBehaviourSet;
    public Team Team { get { return data.team; } }


    
    private float bsTimer = 0;

    //private Vector2 acumulatedPush;
    //public void Push(Vector2 directionNormalized, float force)
    //{
    //    if (acumulatedPush.sqrMagnitude < force * force)
    //    {
    //        acumulatedPush = directionNormalized * force;
    //    }
    //}


    public Rigidbody2D body;

    private void StartUpVariables()
    {
        comparationPosition = transform.position;
        movementDelta = float.MaxValue;
    }
    private void SetStuckDestination()
    {
        Vector2 position = transform.position;
        Vector2 dir = (Destination - position).normalized;
        int rndm = Random.Range(0, 1);
        Vector2 perpendicular = rndm == 0 ? Vector2.Perpendicular(dir) : -Vector2.Perpendicular(dir);

        Vector2 temp = position + perpendicular * data.stuckDesinationOffset;
        RaycastHit2D hit = Physics2D.Linecast(position, temp, 1 << 9);// obstacle layer_____________________________hard coded
        if (hit.collider != null)
            stuckDestination = position - perpendicular * data.stuckDesinationOffset;
        else
            stuckDestination = temp;
    }
    private void Update()
    {
        if (test)
        {
            if(body == null)
            {
                float delta = Time.deltaTime;
                transform.position = ActiveBehaviourSet.CalculateDesiredPosition(this, delta);

                UpdateTimers(delta);
                UpdateVelocityParams(delta);
            }
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

            if (debug) Debug.Log($"Stuck: {Stuck.ToString()}|Acting: {Acting.ToString()}"); //Debug.Log(Velocity.ToString());
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
        bsTimer += deltaTime;
        repathTimer += deltaTime;
        obstacleCicles++;
        movementDeltaTimer += deltaTime;
    }
    private void Start()
    {
        //started = true;
        body = GetComponent<Rigidbody2D>();
        GetAndSetAI();
        StartUpVariables();

        prevPosition1 = prevPosition2 = transform.position;
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
}
