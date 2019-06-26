using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{



    public AIUnit parent;

    public AIAgentData data;

    
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
