using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EntityAction : MonoBehaviour
{
    //no es tan bueno que sepa la info especifica del agente.
    // es posible que una estructura tambien actúe.
    //ej: una torreta que ataca a enemigos

    protected AIAgent agent;
    protected AIAgentData data;
    protected WaitForSeconds wait;
    public Action ActionEnded = delegate { }; 
   

    protected virtual void Awake()
    {
        agent = GetComponent<AIAgent>();
        data = agent.data;
        wait = new WaitForSeconds(1f / data.actionSpeed);        
    }

    protected virtual void OnEnable()
    {
        agent.ActionBegan += ExecuteAction;
    }
    protected virtual void OnDisable()
    {
        agent.ActionBegan -= ExecuteAction;
    }

    protected virtual void ExecuteAction(IEntity target)
    {
        StartCoroutine(NotificateEndOfAction());
    }

    protected IEnumerator NotificateEndOfAction()
    {
        yield return wait;
        ActionEnded();
    }
}
