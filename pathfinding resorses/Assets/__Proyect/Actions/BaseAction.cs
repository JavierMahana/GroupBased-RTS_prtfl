using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(IActable))]
public abstract class BaseAction : MonoBehaviour
{

    protected IActable actor;

    protected WaitForSeconds wait;
    public abstract BaseActionData BaseData { get; }
    public Action ActionEnded = delegate { };


    protected virtual void Awake()
    {
        actor = GetComponent<IActable>();
        wait = new WaitForSeconds(1f / BaseData.actionSpeed);
    }

    protected virtual void OnEnable()
    {
        actor.OnActionStart += ExecuteAction;
    }
    protected virtual void OnDisable()
    {
        actor.OnActionStart -= ExecuteAction;
    }

    protected virtual void ExecuteAction(ITargetable target)
    {
        StartCoroutine(NotificateEndOfAction());
    }
    protected IEnumerator NotificateEndOfAction()
    {
        yield return wait;
        ActionEnded();
    }
}
