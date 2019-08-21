using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//quizas las estructuras podrian heredar de una interfaz que controle el ocupar espacios en el grafico de movimiento.
[RequireComponent(typeof(Health))]
public abstract class Structure : Entity, IMWRUser, ISelectable
{
    public event SelectionEvent SelectionStateChanged = delegate { };

    //used by the MWR
    public static event Action<Structure> OnSpawn = delegate { };
    public static event Action<Structure> OnDeath = delegate { };
    
    public StructureData data;
    public Team team;

    private Health health;
    private bool safeEventInvoke;

    
    protected virtual void Awake()
    {
        safeEventInvoke = false;        
        health = GetComponent<Health>();
    }
    protected virtual void Start()
    {
        StartCoroutine(CallLateUpdate());
    }
    private IEnumerator CallLateUpdate()
    {
        yield return null;
        LateStart();
    }

    private void LateStart()
    {       
        safeEventInvoke = true;
        OnSpawn(this);
    }
    protected virtual void OnEnable()
    {
        Health.InvokeDeath += Death;
        if (safeEventInvoke)
            OnSpawn(this);
    }
    protected virtual void OnDisable()
    {
        Health.InvokeDeath -= Death;
        if (safeEventInvoke)
            OnDeath(this);
    }

    public override float Radious { get { return data.radious; } }
    public override Shape BodyShape { get { return data.bodyShape; } }
    public override GameObject GameObject { get { return gameObject; } }
    public override Team Team { get { return team; } }
    public override Health Health { get { return health; } }

    public Vector2Int CurrentCoordintates { get; set; }
    public MovementWorldRepresentation MWR { get; set; }
    public abstract UIMode UIMode { get; }

    private void Death()
    {

    }
    
    public virtual void Select(SelectionManager manager)
    {
        Debug.Log($"selected {this}");
    }
    public virtual void Deselect(SelectionManager manager)
    {
        Debug.Log($"deselect {this}");
    }

    public ISelectable GetSelectable()
    {
        return this;
    }
}
