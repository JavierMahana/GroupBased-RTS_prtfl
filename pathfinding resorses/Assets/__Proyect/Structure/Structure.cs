using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


//quizas las estructuras podrian heredar de una interfaz que controle el ocupar espacios en el grafico de movimiento.
[RequireComponent(typeof(Health))]
public abstract class Structure : SerializedMonoBehaviour, IMWRUser, ITargetable
{
    public IKillable KillableEntity { get { return this; } }


    public event KillableEvent OnDeath = delegate { };
    //public event KillableEvent Onspawn = delegate { };
    //used by the MWR
    public static event Action<Structure> OnSpawn = delegate { };
    //public static event Action<Structure> OnDeath = delegate { };
    
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
        if (safeEventInvoke)
            OnSpawn(this);
    }
    protected virtual void OnDisable()
    {
        if(!quit)
            OnDeath(this);
    }
    private bool quit;
    protected virtual void OnApplicationQuit()
    {
        quit = true;
    }
    public float Radious { get { return data.radious; } }
    public Shape BodyShape { get { return data.bodyShape; } }
    public GameObject GameObject { get { return gameObject; } }
    public Team Team { get { return team; } }
    public Health Health { get { return health; } }

    public Vector2Int CurrentCoordintates { get; set; }
    public MovementWorldRepresentation MWR { get; set; }
    


    void IDamagable.RecieveDamage(int attackStrenght)
    {
        Health.RecieveDamage(attackStrenght);
    }
}
