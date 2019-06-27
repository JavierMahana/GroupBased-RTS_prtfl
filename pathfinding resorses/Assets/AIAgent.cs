using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class AIAgent : SerializedMonoBehaviour, IEntity
{
    public bool test;

    public AIUnit parent;
    
    public IEntity target;
    public IAstarAI AI { get; private set; }

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

    public AIAgentData data;

    public IBehaviourSet ActiveBehaviourSet;

    private void Update()
    {
        if (test)
        {
            transform.position = ActiveBehaviourSet.CalculateDesiredPosition(this);
        }

    }
    private void Start()
    {
        GetAndSetAI();
    }
    private void GetAndSetAI()
    {
        AI = GetComponent<IAstarAI>();
        AI.canMove = false;
        AI.radius = data.radious;
        AI.maxSpeed = data.maxSpeed;

        if (AI is AIPath)
        {
            //nose como hacerlo la verdad... uwu
        }
        //hacer que los parametros dentro de la IA coincidan con los que se especifican acá.
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
