using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EntityAction : MonoBehaviour
{
    //no es tan bueno que sepa la info especifica del agente.
    // es posible que una estructura tambien actúe.
    //ej: una torreta que ataca a enemigos

    protected Entity entity;
    
    protected WaitForSeconds wait;
    public abstract BaseActionData BaseData { get;}
    public Action ActionEnded = delegate { }; 
   

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();        
        wait = new WaitForSeconds(1f / BaseData.actionSpeed);        
    }

    protected virtual void OnEnable()
    {
        entity.ActionBegan += ExecuteAction;
    }
    protected virtual void OnDisable()
    {
        entity.ActionBegan -= ExecuteAction;
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
