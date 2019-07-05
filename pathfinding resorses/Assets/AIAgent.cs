using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class AIAgent : SerializedMonoBehaviour, IEntity
{
    public bool debug;
    public bool gizmos;

    public bool test;

    public AIUnit parent;
    
    public IEntity target = null;//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
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


    //agruegar timer :v
    public AIAgent[] ObstaclesToEvade
    {
        get
        {
            List<AIAgent> posibleObstacles = parent.ClosestUnit.children;

            AIAgent closestSiblin = null;
            float closestSiblinSqrDistance = float.MaxValue;

            AIAgent secondClosestSiblin = null;
            float secondClosestSiblinSqrDistance = float.MaxValue;
            for (int i = 0; i < posibleObstacles.Count; i++)
            {
                AIAgent tempAgent = posibleObstacles[i];

                if (tempAgent == this || tempAgent == target)
                    continue;

                float tempSqrDistance = Vector2Utilities.SqrDistance(transform.position, tempAgent.transform.position);
                if (tempSqrDistance < closestSiblinSqrDistance)
                {
                    secondClosestSiblin = closestSiblin;
                    secondClosestSiblinSqrDistance = closestSiblinSqrDistance;

                    closestSiblin = tempAgent;
                    closestSiblinSqrDistance = tempSqrDistance;
                }
                else if (tempSqrDistance < secondClosestSiblinSqrDistance)
                {
                    secondClosestSiblin = closestSiblin;
                    secondClosestSiblinSqrDistance = closestSiblinSqrDistance;
                }
            }
            return new AIAgent[2] { closestSiblin, secondClosestSiblin };
        }
    }

    public AIAgentData data;
    

    public IBehaviourSet ActiveBehaviourSet
    {
        get
        {
            if (bsTimer >= BS_UPDATE_TIME || activeBehaviourSet == null)
            {
                activeBehaviourSet = parent.ActiveMacro.GetBehaviourSet(this);
                bsTimer = 0;
            }
                

            return activeBehaviourSet;
        }
    }
    private IBehaviourSet activeBehaviourSet;
    public float BS_UPDATE_TIME = 0.2F; //hacer private constante luego de probar un tiempo bueno
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
    private void Update()
    {
        if (test)
        {
            if(body == null)
            {
                transform.position = ActiveBehaviourSet.CalculateDesiredPosition(this);
                bsTimer += Time.deltaTime;
                repathTimer += Time.time;
            }
        }

    }
    private void FixedUpdate()
    {
        if (body != null)
        {
            body.MovePosition(ActiveBehaviourSet.CalculateDesiredPosition(this));
            //body.position = (ActiveBehaviourSet.CalculateDesiredPosition(this));
            bsTimer += Time.deltaTime;
            repathTimer += Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        //if (test)
        //{
        //    Vector2 newPos = (Vector2)transform.position + acumulatedPush;
        //    transform.position = newPos;
        

        //    acumulatedPush = Vector2.zero;
        //}
    }



    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        GetAndSetAI();
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
        if (gizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, data.radious * data.separationRangeInRadious);
        }
    }

    //public float pipelineWaitTime = 5f;
    //private float passedTime = 0;


    //private IBehaviourSet currentBS;
    //private Rigidbody body;
    //private void Start()
    //{
    //    currentBS = GetInitialBS();
    //    body = GetComponent<Rigidbody>();
    //}
    //private void Update()
    //{
    //    passedTime += Time.deltaTime;

    //    if (pipelineWaitTime <= passedTime)
    //    {
    //        UpdateBehaviourSet();
    //        passedTime = 0;
    //    }
    //}
    //private void FixedUpdate()
    //{
    //    if (currentBS != null && body != null)
    //    {
    //        Vector3 desiredPosition = currentBS.GetDesiredPosition(this);
    //        body.MovePosition(desiredPosition);
    //    }
    //}

    //public IBehaviourSet UpdateBehaviourSet()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public IBehaviourSet GetInitialBS()
    //{
    //    throw new System.NotImplementedException();
    //}
}
