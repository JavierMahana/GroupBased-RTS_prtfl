using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//quizas las estructuras podrian heredar de una interfaz que controle el ocupar espacios en el grafico de movimiento.
[RequireComponent(typeof(Health))]
public class Structure : Entity, IMWRUser
{
    public static event Action<Structure> OnSpawn = delegate { };
    public static event Action<Structure> OnDeath = delegate { };
    
    public StructureData data;
    public Team team;
    public Vector2Int gridCoordinate;
    private Health health;

    private bool safeEventInvoke;
    private void Start()
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
        Debug.Log("late start. Calling the functions");
        safeEventInvoke = true;
        OnSpawn(this);
    }
    private void OnEnable()
    {
        if (safeEventInvoke)
            OnSpawn(this);
    }
    private void OnDisable()
    {
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



    private void Awake()
    {
        safeEventInvoke = false;
        health = GetComponent<Health>();
    }
}
